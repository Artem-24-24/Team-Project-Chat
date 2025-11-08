using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
   
    public partial class Form1 : Form
    {
        private TcpListener listener;             // Об’єкт, який слухає підключення клієнтів
        private Thread listenThread;              // Потік, який приймає клієнтів
        private List<TcpClient> clients = new();  // Список усіх підключених клієнтів
        private bool running = false;             // Статус роботи сервера
        private LocalDatabase _db = new LocalDatabase();
        private const string DbFilePath = "local_db.json";
        private int _currentUserId = -1;


        public Form1()
        {
            InitializeComponent();

            LoadDatabase();

            lblStatus.Text = "Status: Disconnected";
            label2.Text = $"Local DB: {DbFilePath}";
            txtPort.Text = "��������";

            btnStart.Click += BtnStart_Click;
            btnClearLog.Click += BtnClearLog_Click;
            btnStop.Click += BtnStop_Click;
            btnRefresh.Click += BtnRefresh_Click;
        }

        #region UI Logic and Helpers

        private void Log(string message)
        {
            if (listBoxLog.InvokeRequired)
            {
                listBoxLog.Invoke(new Action(() => listBoxLog.Items.Add($"[{DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy")}] {message}")));
            }
            else
            {
                listBoxLog.Items.Add($"[{DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy")}] {message}");
            }
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            _currentUserId = -1;
            lblStatus.Text = "Status: Disconnected";
            Log("����� ��������� (������� ������).");
        }


        private async void BtnStart_Click(object sender, EventArgs e) // Vlad
        {
            if (_currentUserId != -1)
            {
                Log("��� �������������! ������ ����� ��������� ������!");
                return;
            }

            var username = "testuser";
            var password = "password123";

            Log($"������ ����� ��� {username}...");

            if (Login(username, password))
            {
                lblStatus.Text = "Status: Logged In";
                Log($"������� ����. User ID: {_currentUserId}");
                await TestChatEndpoints();
                return;
            }

            Log("�� ������ �����������. ������ ���������...");
            if (Register(username, password))
            {
                Login(username, password);
                lblStatus.Text = "Status: Registered & Logged In";
                Log($"������ ��������� � ����. User ID: {_currentUserId}");
                await TestChatEndpoints();
                return;
            }

            lblStatus.Text = "Status: Failed to Connect/Login";
            Log("�������: �� ������� ����� ��� ��������������.");
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (_currentUserId == -1)
            {
                Log("�������� �������������.");
                return;
            }

            DisplayAllUsers();
        }

        #endregion

        #region Database and Persistence

        private void LoadDatabase()
        {
            if (File.Exists(DbFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(DbFilePath);
                    _db = JsonSerializer.Deserialize<LocalDatabase>(jsonString) ?? new LocalDatabase();
                    Log("���� ����� ������ �����������!");
                }
                catch (Exception ex)
                {
                    Log($"������� ������������ ��: {ex.Message}. �������������� ����� ��!");
                    _db = new LocalDatabase();
                }
            }
            else
            {
                Log("���� �� �� ���������. ���������� ���� ���� �����.");
            }
            UpdateClientGrid();
        }

        private void SaveDatabase()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string jsonString = JsonSerializer.Serialize(_db, options);
                File.WriteAllText(DbFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Log($"������� ���������� ��: {ex.Message}");
            }
        }

        private void UpdateClientGrid()
        {
            dataGridClients.DataSource = null;
            dataGridClients.DataSource = _db.Users.Select(u => new
            {
                u.Id,
                u.Username,
                Status = u.Id == _currentUserId ? "Me (Logged In)" : "Available"
            }).ToList();
            lblClients.Text = $"Clients connected: {_db.Users.Count}";
        }

        private void DisplayAllUsers()
        {
            Log($"������ ������������ ({_db.Users.Count})");
            foreach (var user in _db.Users)
            {
                Log($"ID: {user.Id}, Username: {user.Username}");
            }
            UpdateClientGrid();
        }

        #endregion

        #region API Implementation

        private bool Register(string username, string password)
        {
            if (_db.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Log("������� ���������: ���������� ��� ����.");
                return false;
            }

            var newUser = new UserModel
            {
                Id = _db.NextUserId,
                Username = username,
                PasswordHash = password
            };

            _db.Users.Add(newUser);
            SaveDatabase();
            return true;
        }

        private bool Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == password);

            if (user != null)
            {
                _currentUserId = user.Id;
                return true;
            }

            return false;
        }

        private List<MessageModel> GetMessageHistory(int contactId)
        {
            if (_currentUserId == -1) return new List<MessageModel>();

            var history = _db.Messages
                .Where(m => (m.SenderId == _currentUserId && m.ReceiverId == contactId) ||
                            (m.SenderId == contactId && m.ReceiverId == _currentUserId))
                .OrderBy(m => DateTime.ParseExact(m.Timestamp, "HH:mm:ss dd.MM.yyyy", null))
                .ToList();

            return history;
        }

        private MessageModel SendMessage(int receiverId, string content)
        {
            if (_currentUserId == -1)
            {
                Log("�������: �� ����� ��������� ����������� ��� �����������!");
                return null;
            }

            var newMessage = new MessageModel
            {
                Id = _db.NextMessageId,
                SenderId = _currentUserId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy")
            };

            _db.Messages.Add(newMessage);
            SaveDatabase();
            return newMessage;
        }

        #endregion

        #region Test Endpoints

        private Task TestChatEndpoints()
        {
            if (_db.Users.FirstOrDefault(u => u.Id == 2) == null)
            {
                Register("contactuser", "pass2");
            }

            int contactId = 2;

            Log($"���� API: ³������� ����������� ID {contactId}");
            SendMessage(contactId, "�����, �� ������� ����������� �� Form1!");

            Log($"���� API: ��������� ������� � ID {contactId}");
            var history = GetMessageHistory(contactId);

            foreach (var msg in history)
            {
                Log($"����������� �� {msg.SenderId} � {msg.Timestamp}: {msg.Content}");
            }

            //Log("���� API ���������");

            return Task.CompletedTask;
        }

        #endregion

        private void txtPort_TextChanged(object sender, EventArgs e)
        {
        }

        //Кнопка "Start" — запуск сервера
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                // Створюємо TCP-слухача на порту 9000
                listener = new TcpListener(IPAddress.Any, 9000);
                listener.Start();
                running = true;

                // Запускаємо потік для прослуховування підключень
                listenThread = new Thread(ListenForClients);
                listenThread.Start();

                LogChat("Server started, Port: 9000");
            }
        }

        // Метод, який постійно чекає підключення нових клієнтів
        private void ListenForClients()
        {
            try
            {
                while (running)
                {
                    // Приймаємо нове підключення
                    TcpClient client = listener.AcceptTcpClient();
                    clients.Add(client);
                    UpdateClientCount();

                    // Створюємо новий потік для роботи з цим клієнтом
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);

                    LogChat("New client connected");
                }
            }
            catch (SocketException ex)
            {
                LogChat($"Server was stopped: {ex.Message}");
            }
        }

        // Обробка повідомлень від конкретного клієнта
        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            int? authenticatedUserId = null;
            string authenticatedUsername = "";

            while (running)
            {
                try
                {
                    // Зчитуємо повідомлення
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    // Try to parse as protocol message
                    try
                    {
                        var protocolMsg = JsonSerializer.Deserialize<ProtocolMessage>(msg);
                        
                        if (protocolMsg != null)
                        {
                            if (protocolMsg.Type == "login")
                            {
                                LogChat($"Login attempt: {protocolMsg.Username}");
                                var response = new ProtocolMessage { Type = "response" };
                                
                                if (Login(protocolMsg.Username, protocolMsg.Password))
                                {
                                    var user = _db.Users.FirstOrDefault(u => u.Username.Equals(protocolMsg.Username, StringComparison.OrdinalIgnoreCase));
                                    authenticatedUserId = user.Id;
                                    authenticatedUsername = user.Username;
                                    response.Success = true;
                                    response.UserId = user.Id;
                                    response.Username = user.Username;
                                    LogChat($"Login successful: {protocolMsg.Username} (ID: {user.Id})");
                                    UpdateClientGrid();
                                }
                                else
                                {
                                    response.Success = false;
                                    response.ErrorMessage = "Invalid username or password";
                                    LogChat($"Login failed: {protocolMsg.Username}");
                                }
                                
                                SendProtocolMessage(stream, response);
                            }
                            else if (protocolMsg.Type == "register")
                            {
                                LogChat($"Register attempt: {protocolMsg.Username}");
                                var response = new ProtocolMessage { Type = "response" };
                                
                                if (Register(protocolMsg.Username, protocolMsg.Password))
                                {
                                    var user = _db.Users.FirstOrDefault(u => u.Username.Equals(protocolMsg.Username, StringComparison.OrdinalIgnoreCase));
                                    authenticatedUserId = user.Id;
                                    authenticatedUsername = user.Username;
                                    response.Success = true;
                                    response.UserId = user.Id;
                                    response.Username = user.Username;
                                    LogChat($"Registration successful: {protocolMsg.Username} (ID: {user.Id})");
                                    UpdateClientGrid();
                                }
                                else
                                {
                                    response.Success = false;
                                    response.ErrorMessage = "Username already exists";
                                    LogChat($"Registration failed: {protocolMsg.Username}");
                                }
                                
                                SendProtocolMessage(stream, response);
                            }
                            else if (protocolMsg.Type == "chat" && authenticatedUserId.HasValue)
                            {
                                // Chat message from authenticated user
                                string formattedMsg = $"{authenticatedUsername}: {protocolMsg.Content}";
                                LogChat($"<{authenticatedUsername}>: {protocolMsg.Content}");
                                
                                // Save message to database (broadcast message, receiverId = 0 for all)
                                var chatMsg = new MessageModel
                                {
                                    Id = _db.NextMessageId,
                                    SenderId = authenticatedUserId.Value,
                                    ReceiverId = 0, // 0 means broadcast
                                    Content = protocolMsg.Content,
                                    Timestamp = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy")
                                };
                                _db.Messages.Add(chatMsg);
                                SaveDatabase();
                                
                                // Відправляємо повідомлення іншим клієнтам
                                Broadcast(formattedMsg, client);
                            }
                        }
                    }
                    catch
                    {
                        // If not a protocol message, treat as regular chat (legacy support)
                        if (authenticatedUserId.HasValue)
                        {
                            LogChat($"<{authenticatedUsername}>: {msg}");
                            Broadcast(msg, client);
                        }
                    }
                }
                catch
                {
                    break;
                }
            }

            // Якщо клієнт відключився
            clients.Remove(client);
            UpdateClientCount();
            if (authenticatedUsername != "")
            {
                LogChat($"Client disconnected: {authenticatedUsername}");
            }
        }

        private void SendProtocolMessage(NetworkStream stream, ProtocolMessage message)
        {
            try
            {
                string json = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                LogChat($"Error sending protocol message: {ex.Message}");
            }
        }

        // Надсилання повідомлення всім підключеним клієнтам (крім відправника)
        private void Broadcast(string msg, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);

            foreach (var client in clients)
            {
                if (client != sender)
                {
                    try
                    {
                        client.GetStream().Write(data, 0, data.Length);
                    }
                    catch
                    {
                        // Ігноруємо, якщо клієнт уже розірвав з’єднання
                    }
                }
            }
        }

        // Додає запис у список логів на формі
        private void LogChat(string text)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => listBoxLog.Items.Add(text)));
            else
                listBoxLog.Items.Add(text);
        }

        // Оновлює лічильник підключених клієнтів на формі
        private void UpdateClientCount()
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => lblClients.Text = $"Clients: {clients.Count}"));
            else
                lblClients.Text = $"Clietns: {clients.Count}";
        }

        // Кнопка "Stop" — зупинка сервера
        private void btnStop_Click(object sender, EventArgs e)
        {
            running = false;
            listener.Stop();
            clients.Clear();
            Log("Server was stopped");
            UpdateClientCount();
        }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }

    public class MessageModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public string Timestamp { get; set; }
    }

    public class LocalDatabase
    {
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public int NextUserId => Users.Any() ? Users.Max(u => u.Id) + 1 : 1;
        public int NextMessageId => Messages.Any() ? Messages.Max(m => m.Id) + 1 : 1;
    }

    // Protocol messages for client-server communication
    public class ProtocolMessage
    {
        public string Type { get; set; } // "login", "register", "chat", "response"
        public string Username { get; set; }
        public string Password { get; set; }
        public string Content { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int UserId { get; set; }
    }
}