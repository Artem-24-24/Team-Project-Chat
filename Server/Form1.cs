using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Protocol;

namespace Server
{
    public partial class Form1 : Form
    {
        private TcpListener listener;
        private Thread listenThread;
        private List<TcpClient> clients = new();
        private Dictionary<TcpClient, int> clientSessions = new Dictionary<TcpClient, int>();
        private bool running = false;
        private LocalDatabase _db = new LocalDatabase();
        private const string DbFilePath = "database.json";
        private int _currentUserId = -1;
        private Dictionary<string, List<Protocol.ChatMessage>> messageHistory = new Dictionary<string, List<Protocol.ChatMessage>>();

        public Form1()
        {
            InitializeComponent();
            LoadDatabase();

            lblStatus.Text = "Статус: Зупинено";
            label2.Text = $"Локальна БД: database.json";
            txtPort.Text = "9000";

            btnStart.Enabled = true;
            btnStop.Enabled = false;

            btnStart.Click += btnStart_Click;
            btnClearLog.Click += BtnClearLog_Click;
            btnStop.Click += btnStop_Click;
            btnRefresh.Click += BtnRefresh_Click;
        }

        private void Log(string message)
        {
            if (listBoxLog.InvokeRequired)
            {
                listBoxLog.Invoke(new Action(() => listBoxLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}")));
            }
            else
            {
                listBoxLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            UpdateClientGrid();
            DisplayAllUsers();
        }

        private void LoadDatabase()
        {
            if (File.Exists(DbFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(DbFilePath);
                    _db = JsonSerializer.Deserialize<LocalDatabase>(jsonString) ?? new LocalDatabase();
                    Log($"Базу даних завантажено успішно! Знайдено користувачів: {_db.Users.Count}");

                    foreach (var user in _db.Users)
                    {
                        Log($"Завантажено користувача: ID={user.Id}, Username={user.Username}");
                    }

                    LoadMessageHistoryFromDatabase();
                }
                catch (Exception ex)
                {
                    Log($"Помилка завантаження БД: {ex.Message}. Використовується порожня БД!");
                    _db = new LocalDatabase();
                }
            }
            else
            {
                Log("Файл БД не знайдено. Створюється нова база даних.");
                SaveDatabase();
            }
            UpdateClientGrid();
        }

        private void LoadMessageHistoryFromDatabase()
        {
            messageHistory.Clear();

            foreach (var message in _db.Messages)
            {
                var senderUser = _db.Users.FirstOrDefault(u => u.Id == message.SenderId);
                var receiverUser = _db.Users.FirstOrDefault(u => u.Id == message.ReceiverId);

                if (senderUser != null && receiverUser != null)
                {
                    var chatMessage = new Protocol.ChatMessage
                    {
                        Username = senderUser.Username,
                        Content = message.Content,
                        Timestamp = message.Timestamp,
                        Recipient = receiverUser.Username
                    };

                    string key = GetMessageHistoryKey(senderUser.Username, receiverUser.Username);
                    if (!messageHistory.ContainsKey(key))
                        messageHistory[key] = new List<Protocol.ChatMessage>();

                    messageHistory[key].Add(chatMessage);
                }
            }

            Log($"Завантажено {_db.Messages.Count} повідомлень з бази даних");
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
                Log($"Базу даних збережено успішно. Користувачів: {_db.Users.Count}, Повідомлень: {_db.Messages.Count}");
            }
            catch (Exception ex)
            {
                Log($"Помилка збереження БД: {ex.Message}");
            }
        }

        private void UpdateClientGrid()
        {
            if (dataGridClients.InvokeRequired)
            {
                dataGridClients.Invoke(new Action(() =>
                {
                    dataGridClients.DataSource = null;
                    dataGridClients.DataSource = _db.Users.Select(u => new
                    {
                        u.Id,
                        u.Username,
                        Status = clientSessions.Values.Contains(u.Id) ? "Онлайн" : "Офлайн"
                    }).ToList();
                }));
            }
            else
            {
                dataGridClients.DataSource = null;
                dataGridClients.DataSource = _db.Users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    Status = clientSessions.Values.Contains(u.Id) ? "Онлайн" : "Офлайн"
                }).ToList();
            }

            UpdateClientCount();
        }

        private void UpdateClientCount()
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => lblClients.Text = $"Клієнти: {clientSessions.Count}"));
            else
                lblClients.Text = $"Клієнти: {clientSessions.Count}";
        }

        private void DisplayAllUsers()
        {
            Log($"Список користувачів ({_db.Users.Count}):");
            foreach (var user in _db.Users)
            {
                string status = clientSessions.Values.Contains(user.Id) ? "Онлайн" : "Офлайн";
                Log($"ID: {user.Id}, Username: {user.Username}, Статус: {status}");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                try
                {
                    int port = int.Parse(txtPort.Text.Trim());
                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                    running = true;

                    listenThread = new Thread(ListenForClients);
                    listenThread.IsBackground = true;
                    listenThread.Start();

                    Log($"Сервер запущено на порту {port}");
                    lblStatus.Text = "Статус: Запущено";

                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    txtPort.Enabled = false;
                }
                catch (Exception ex)
                {
                    Log($"Помилка запуску сервера: {ex.Message}");
                    running = false;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (running)
            {
                running = false;

                try
                {
                    listener?.Stop();

                    foreach (var client in clients.ToList())
                    {
                        try
                        {
                            client?.Close();
                        }
                        catch (Exception ex)
                        {
                            Log($"Помилка закриття клієнта: {ex.Message}");
                        }
                    }

                    if (listenThread != null && listenThread.IsAlive)
                    {
                        if (!listenThread.Join(2000))
                        {
                            Log("Примусове завершення потоку прослуховування");
                            listenThread.Abort();
                        }
                    }

                    clients.Clear();
                    clientSessions.Clear();

                    Log("Сервер зупинено");
                    lblStatus.Text = "Статус: Зупинено";

                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    txtPort.Enabled = true;

                    UpdateClientCount();
                    UpdateClientGrid();
                }
                catch (Exception ex)
                {
                    Log($"Помилка при зупинці сервера: {ex.Message}");
                }
            }
        }

        private void ListenForClients()
        {
            try
            {
                while (running)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        clients.Add(client);

                        Thread clientThread = new Thread(HandleClient);
                        clientThread.IsBackground = true;
                        clientThread.Start(client);

                        Log("Новий клієнт підключився");
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (SocketException ex)
                    {
                        if (running)
                        {
                            Log($"Помилка сокета: {ex.Message}");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (running)
                    Log($"Помилка сервера: {ex.Message}");
            }

            Log("Потік прослуховування завершено");
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (running && client.Connected)
                {
                    try
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        var networkMessage = JsonSerializer.Deserialize<NetworkMessage>(jsonData);

                        switch (networkMessage.Type)
                        {
                            case MessageType.Login:
                                HandleLogin(client, networkMessage.Data);
                                break;
                            case MessageType.Register:
                                HandleRegister(client, networkMessage.Data);
                                break;
                            case MessageType.Chat:
                                HandleChatMessage(client, networkMessage.Data);
                                break;
                            case MessageType.Disconnect:
                                HandleDisconnect(client);
                                return;
                            case MessageType.UserListRequest:
                                SendUserList(client);
                                break;
                            case MessageType.PrivateMessage:
                                var privateMsg = JsonSerializer.Deserialize<Protocol.ChatMessage>(networkMessage.Data);
                                HandlePrivateMessage(privateMsg, client);
                                break;
                            case MessageType.MessageHistoryRequest:
                                var historyRequest = JsonSerializer.Deserialize<Protocol.MessageHistoryRequest>(networkMessage.Data);
                                SendMessageHistory(historyRequest, client);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (running && client.Connected)
                        {
                            Log($"Помилка обробки клієнта: {ex.Message}");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (running)
                {
                    Log($"Помилка клієнтського потоку: {ex.Message}");
                }
            }
            finally
            {
                try
                {
                    stream?.Close();
                }
                catch { }

                HandleDisconnect(client);
            }
        }

        private void HandleLogin(TcpClient client, string data)
        {
            try
            {
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(data);
                var response = new LoginResponse();

                var user = _db.Users.FirstOrDefault(u =>
                    u.Username.Equals(loginRequest.Username, StringComparison.OrdinalIgnoreCase) &&
                    u.PasswordHash == loginRequest.Password);

                if (user != null)
                {
                    if (clientSessions.Values.Contains(user.Id))
                    {
                        response.Success = false;
                        response.Message = "Користувач уже увійшов у систему";
                    }
                    else
                    {
                        response.Success = true;
                        response.Message = "Вхід виконано успішно";
                        response.UserId = user.Id;
                        clientSessions[client] = user.Id;
                        Log($"Користувач {loginRequest.Username} увійшов у систему");
                        UpdateClientGrid();
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Невірні облікові дані";
                    Log($"Неуспішна спроба входу: {loginRequest.Username}");
                }

                SendToClient(client, MessageType.Login, JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                Log($"Помилка обробки входу: {ex.Message}");
                var errorResponse = new LoginResponse
                {
                    Success = false,
                    Message = "Помилка сервера при обробці входу"
                };
                SendToClient(client, MessageType.Login, JsonSerializer.Serialize(errorResponse));
            }
        }

        private void HandleRegister(TcpClient client, string data)
        {
            try
            {
                var registerRequest = JsonSerializer.Deserialize<LoginRequest>(data);
                var response = new LoginResponse();

                if (string.IsNullOrWhiteSpace(registerRequest.Username) || string.IsNullOrWhiteSpace(registerRequest.Password))
                {
                    response.Success = false;
                    response.Message = "Ім'я користувача та пароль не можуть бути порожніми";
                }
                else if (_db.Users.Any(u => u.Username.Equals(registerRequest.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    response.Success = false;
                    response.Message = "Користувач вже існує";
                    Log($"Спроба реєстрації існуючого користувача: {registerRequest.Username}");
                }
                else
                {
                    var newUser = new UserModel
                    {
                        Id = _db.NextUserId,
                        Username = registerRequest.Username,
                        PasswordHash = registerRequest.Password
                    };

                    _db.Users.Add(newUser);
                    SaveDatabase();

                    response.Success = true;
                    response.Message = "Реєстрація виконана успішно";
                    response.UserId = newUser.Id;
                    clientSessions[client] = newUser.Id;
                    Log($"Користувач {registerRequest.Username} зареєструвався");
                    UpdateClientGrid();
                }

                SendToClient(client, MessageType.Register, JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                Log($"Помилка обробки реєстрації: {ex.Message}");
                var errorResponse = new LoginResponse
                {
                    Success = false,
                    Message = "Помилка сервера при обробці реєстрації"
                };
                SendToClient(client, MessageType.Register, JsonSerializer.Serialize(errorResponse));
            }
        }

        private void HandleChatMessage(TcpClient client, string data)
        {
            if (!clientSessions.ContainsKey(client)) return;

            var chatMessage = JsonSerializer.Deserialize<ChatMessage>(data);
            var userId = clientSessions[client];
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                chatMessage.Username = user.Username;
                chatMessage.Timestamp = DateTime.Now.ToString("HH:mm:ss");

                Log($"<{chatMessage.Username}>: {chatMessage.Content}");

                BroadcastChatMessage(chatMessage, client);
            }
        }

        private void HandleDisconnect(TcpClient client)
        {
            if (clientSessions.ContainsKey(client))
            {
                var userId = clientSessions[client];
                var user = _db.Users.FirstOrDefault(u => u.Id == userId);
                Log($"Користувач {user?.Username} відключився");
                clientSessions.Remove(client);
                UpdateClientGrid();
            }

            clients.Remove(client);
            UpdateClientCount();
            client?.Close();
        }

        private void SendToClient(TcpClient client, MessageType type, string data)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    var message = new NetworkMessage
                    {
                        Type = type,
                        Data = data
                    };

                    string jsonData = JsonSerializer.Serialize(message);
                    byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                    client.GetStream().Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Log($"Помилка відправки даних клієнту: {ex.Message}");
                HandleDisconnect(client);
            }
        }

        private void BroadcastChatMessage(ChatMessage chatMessage, TcpClient sender)
        {
            string data = JsonSerializer.Serialize(chatMessage);

            foreach (var client in clients.ToList())
            {
                if (client != sender && clientSessions.ContainsKey(client))
                {
                    SendToClient(client, MessageType.Chat, data);
                }
            }
        }

        private void SendUserList(TcpClient client)
        {
            try
            {
                var userList = new Protocol.UserListResponse
                {
                    Users = _db.Users.Select(u => u.Username).ToList()
                };

                Log($"Відправка списку користувачів клієнту: {string.Join(", ", userList.Users)} (всього: {userList.Users.Count})");

                var message = new Protocol.NetworkMessage
                {
                    Type = Protocol.MessageType.UserList,
                    Data = JsonSerializer.Serialize(userList)
                };

                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log($"Помилка відправки списку користувачів: {ex.Message}");
            }
        }

        private void HandlePrivateMessage(Protocol.ChatMessage privateMsg, TcpClient sender)
        {
            try
            {
                Log($"Обробка приватного повідомлення: {privateMsg.Username} -> {privateMsg.Recipient}: {privateMsg.Content}");

                string key = GetMessageHistoryKey(privateMsg.Username, privateMsg.Recipient);
                if (!messageHistory.ContainsKey(key))
                    messageHistory[key] = new List<Protocol.ChatMessage>();

                messageHistory[key].Add(privateMsg);

                var senderUser = _db.Users.FirstOrDefault(u => u.Username == privateMsg.Username);
                var recipientUser = _db.Users.FirstOrDefault(u => u.Username == privateMsg.Recipient);

                if (senderUser != null && recipientUser != null)
                {
                    var messageModel = new MessageModel
                    {
                        Id = _db.NextMessageId,
                        SenderId = senderUser.Id,
                        ReceiverId = recipientUser.Id,
                        Content = privateMsg.Content,
                        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    _db.Messages.Add(messageModel);
                    SaveDatabase();
                    Log($"Повідомлення додано до БД: ID={messageModel.Id}, Від={senderUser.Username}, До={recipientUser.Username}");
                }
                else
                {
                    Log($"Помилка: не знайдено користувача. Відправник: {privateMsg.Username}, Одержувач: {privateMsg.Recipient}");
                }

                TcpClient recipientClient = null;
                foreach (var kvp in clientSessions)
                {
                    if (_db.Users.FirstOrDefault(u => u.Id == kvp.Value)?.Username == privateMsg.Recipient)
                    {
                        recipientClient = kvp.Key;
                        break;
                    }
                }

                if (recipientClient != null)
                {
                    var message = new Protocol.NetworkMessage
                    {
                        Type = Protocol.MessageType.PrivateMessage,
                        Data = JsonSerializer.Serialize(privateMsg)
                    };

                    string jsonData = JsonSerializer.Serialize(message);
                    byte[] data = Encoding.UTF8.GetBytes(jsonData);
                    recipientClient.GetStream().Write(data, 0, data.Length);
                }

                Log($"Приватне повідомлення від {privateMsg.Username} до {privateMsg.Recipient}: {privateMsg.Content}");
            }
            catch (Exception ex)
            {
                Log($"Помилка обробки приватного повідомлення: {ex.Message}");
            }
        }

        private void SendMessageHistory(Protocol.MessageHistoryRequest request, TcpClient client)
        {
            try
            {
                string key = GetMessageHistoryKey(request.Username1, request.Username2);

                if (!messageHistory.ContainsKey(key))
                {
                    var user1 = _db.Users.FirstOrDefault(u => u.Username == request.Username1);
                    var user2 = _db.Users.FirstOrDefault(u => u.Username == request.Username2);

                    if (user1 != null && user2 != null)
                    {
                        var messages = _db.Messages.Where(m =>
                            (m.SenderId == user1.Id && m.ReceiverId == user2.Id) ||
                            (m.SenderId == user2.Id && m.ReceiverId == user1.Id))
                            .OrderBy(m => m.Id)
                            .ToList();

                        messageHistory[key] = new List<Protocol.ChatMessage>();

                        foreach (var msg in messages)
                        {
                            var sender = _db.Users.FirstOrDefault(u => u.Id == msg.SenderId);
                            var receiver = _db.Users.FirstOrDefault(u => u.Id == msg.ReceiverId);

                            if (sender != null && receiver != null)
                            {
                                messageHistory[key].Add(new Protocol.ChatMessage
                                {
                                    Username = sender.Username,
                                    Content = msg.Content,
                                    Timestamp = msg.Timestamp,
                                    Recipient = receiver.Username
                                });
                            }
                        }
                    }
                }

                var history = messageHistory.ContainsKey(key) ? messageHistory[key] : new List<Protocol.ChatMessage>();

                var response = new Protocol.MessageHistoryResponse
                {
                    Messages = history
                };

                var message = new Protocol.NetworkMessage
                {
                    Type = Protocol.MessageType.MessageHistory,
                    Data = JsonSerializer.Serialize(response)
                };

                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                client.GetStream().Write(data, 0, data.Length);

                Log($"Відправлено історію повідомлень між {request.Username1} та {request.Username2}: {history.Count} повідомлень");
            }
            catch (Exception ex)
            {
                Log($"Помилка відправки історії повідомлень: {ex.Message}");
            }
        }

        private string GetMessageHistoryKey(string username1, string username2)
        {
            var users = new[] { username1, username2 }.OrderBy(x => x).ToArray();
            return $"{users[0]}_{users[1]}";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log("Закриття застосунку - зупинка сервера...");
            StopServer();

            Thread.Sleep(500);
        }

        private void TxtPort_TextChanged(object sender, EventArgs e) { }

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