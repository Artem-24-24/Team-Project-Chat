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
        private TcpListener listener;             // РћР±вЂ™С”РєС‚, СЏРєРёР№ СЃР»СѓС…Р°С” РїС–РґРєР»СЋС‡РµРЅРЅСЏ РєР»С–С”РЅС‚С–РІ
        private Thread listenThread;              // РџРѕС‚С–Рє, СЏРєРёР№ РїСЂРёР№РјР°С” РєР»С–С”РЅС‚С–РІ
        private List<TcpClient> clients = new();  // РЎРїРёСЃРѕРє СѓСЃС–С… РїС–РґРєР»СЋС‡РµРЅРёС… РєР»С–С”РЅС‚С–РІ
        private bool running = false;             // РЎС‚Р°С‚СѓСЃ СЂРѕР±РѕС‚Рё СЃРµСЂРІРµСЂР°
        private LocalDatabase _db = new LocalDatabase();
        private const string DbFilePath = "local_db.json";
        private int _currentUserId = -1;


        public Form1()
        {
            InitializeComponent();

            LoadDatabase();

            lblStatus.Text = "Status: Disconnected";
            label2.Text = $"Local DB: {DbFilePath}";
            txtPort.Text = "Локально";

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
            Log("Сессія завершена (имітація виходу).");
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (_currentUserId != -1)
            {
                Log("Вже авторизований! Вийдіть перед повторним входом!");
                return;
            }

            var username = "testuser";
            var password = "password123";

            Log($"Спроба логіну для {username}...");

            if (Login(username, password))
            {
                lblStatus.Text = "Status: Logged In";
                Log($"Успішний вхід. User ID: {_currentUserId}");
                await TestChatEndpoints();
                return;
            }

            Log("Не вийшло залогінитися. Спроба реєстраціЇ...");
            if (Register(username, password))
            {
                Login(username, password);
                lblStatus.Text = "Status: Registered & Logged In";
                Log($"Успішна реєстрація і вхід. User ID: {_currentUserId}");
                await TestChatEndpoints();
                return;
            }

            lblStatus.Text = "Status: Failed to Connect/Login";
            Log("Помилка: Не вдалося ввійти або зареєструватися.");
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (_currentUserId == -1)
            {
                Log("Спочатку авторизуйтесь.");
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
                    Log("База даних успішно завантажена!");
                }
                catch (Exception ex)
                {
                    Log($"Помилка завантаження БД: {ex.Message}. Використовуєтся пуста БД!");
                    _db = new LocalDatabase();
                }
            }
            else
            {
                Log("Файл БД не знайдений. Створюєтся нова база даних.");
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
                Log($"Помилка збереження БД: {ex.Message}");
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
            Log($"Список користувачів ({_db.Users.Count})");
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
                Log("Помилка реєстрації: Користувач вже існує.");
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
                Log("Помилка: Не можна відправити повідомлення без авторизації!");
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

            Log($"Тест API: Відправка повідомлення ID {contactId}");
            SendMessage(contactId, "Привіт, це тестове повідомлення із Form1!");

            Log($"Тест API: Получение истории с ID {contactId}");
            var history = GetMessageHistory(contactId);

            foreach (var msg in history)
            {
                Log($"Повідомлення від {msg.SenderId} в {msg.Timestamp}: {msg.Content}");
            }

            //Log("Тест API завершено");

            return Task.CompletedTask;
        }

        #endregion

        private void txtPort_TextChanged(object sender, EventArgs e)
        {
        }

        //РљРЅРѕРїРєР° "Start" вЂ” Р·Р°РїСѓСЃРє СЃРµСЂРІРµСЂР°
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                // РЎС‚РІРѕСЂСЋС”РјРѕ TCP-СЃР»СѓС…Р°С‡Р° РЅР° РїРѕСЂС‚Сѓ 9000
                listener = new TcpListener(IPAddress.Any, 9000);
                listener.Start();
                running = true;

                // Р—Р°РїСѓСЃРєР°С”РјРѕ РїРѕС‚С–Рє РґР»СЏ РїСЂРѕСЃР»СѓС…РѕРІСѓРІР°РЅРЅСЏ РїС–РґРєР»СЋС‡РµРЅСЊ
                listenThread = new Thread(ListenForClients);
                listenThread.Start();

                LogChat("Server started, Port: 9000");
            }
        }

        // РњРµС‚РѕРґ, СЏРєРёР№ РїРѕСЃС‚С–Р№РЅРѕ С‡РµРєР°С” РїС–РґРєР»СЋС‡РµРЅРЅСЏ РЅРѕРІРёС… РєР»С–С”РЅС‚С–РІ
        private void ListenForClients()
        {
            try
            {
                while (running)
                {
                    // РџСЂРёР№РјР°С”РјРѕ РЅРѕРІРµ РїС–РґРєР»СЋС‡РµРЅРЅСЏ
                    TcpClient client = listener.AcceptTcpClient();
                    clients.Add(client);
                    UpdateClientCount();

                    // РЎС‚РІРѕСЂСЋС”РјРѕ РЅРѕРІРёР№ РїРѕС‚С–Рє РґР»СЏ СЂРѕР±РѕС‚Рё Р· С†РёРј РєР»С–С”РЅС‚РѕРј
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

        // РћР±СЂРѕР±РєР° РїРѕРІС–РґРѕРјР»РµРЅСЊ РІС–Рґ РєРѕРЅРєСЂРµС‚РЅРѕРіРѕ РєР»С–С”РЅС‚Р°
        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (running)
            {
                try
                {
                    // Р—С‡РёС‚СѓС”РјРѕ РїРѕРІС–РґРѕРјР»РµРЅРЅСЏ
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    LogChat($"<Message>: {msg}");

                    // Р’С–РґРїСЂР°РІР»СЏС”РјРѕ РїРѕРІС–РґРѕРјР»РµРЅРЅСЏ С–РЅС€РёРј РєР»С–С”РЅС‚Р°Рј
                    Broadcast(msg, client);
                }
                catch
                {
                    break;
                }
            }

            // РЇРєС‰Рѕ РєР»С–С”РЅС‚ РІС–РґРєР»СЋС‡РёРІСЃСЏ
            clients.Remove(client);
            UpdateClientCount();
            //Log("Client disconnected");
        }

        // РќР°РґСЃРёР»Р°РЅРЅСЏ РїРѕРІС–РґРѕРјР»РµРЅРЅСЏ РІСЃС–Рј РїС–РґРєР»СЋС‡РµРЅРёРј РєР»С–С”РЅС‚Р°Рј (РєСЂС–Рј РІС–РґРїСЂР°РІРЅРёРєР°)
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
                        // Р†РіРЅРѕСЂСѓС”РјРѕ, СЏРєС‰Рѕ РєР»С–С”РЅС‚ СѓР¶Рµ СЂРѕР·С–СЂРІР°РІ Р·вЂ™С”РґРЅР°РЅРЅСЏ
                    }
                }
            }
        }

        // Р”РѕРґР°С” Р·Р°РїРёСЃ Сѓ СЃРїРёСЃРѕРє Р»РѕРіС–РІ РЅР° С„РѕСЂРјС–
        private void LogChat(string text)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => listBoxLog.Items.Add(text)));
            else
                listBoxLog.Items.Add(text);
        }

        // РћРЅРѕРІР»СЋС” Р»С–С‡РёР»СЊРЅРёРє РїС–РґРєР»СЋС‡РµРЅРёС… РєР»С–С”РЅС‚С–РІ РЅР° С„РѕСЂРјС–
        private void UpdateClientCount()
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => lblClients.Text = $"Clients: {clients.Count}"));
            else
                lblClients.Text = $"Clietns: {clients.Count}";
        }

        // РљРЅРѕРїРєР° "Stop" вЂ” Р·СѓРїРёРЅРєР° СЃРµСЂРІРµСЂР°
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
}