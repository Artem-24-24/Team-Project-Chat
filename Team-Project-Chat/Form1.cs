using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Team_Project_Chat
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private string username = "Невідомо";
        private bool isConnected = false;
        private string selectedUser = null;
        private Dictionary<string, List<Protocol.ChatMessage>> messageHistory = new Dictionary<string, List<Protocol.ChatMessage>>();

        public Form1()
        {
            InitializeComponent();
            btnSend.Enabled = false;
            txtMessage.Enabled = false;
            InitializeTimer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            From_Login loginForm = new From_Login();
            var result = loginForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.username = loginForm.GetUsername();
                this.Text = $"Чат - {username}";
                if (lblLoggedIn != null)
                    lblLoggedIn.Text = $"Користувач: {username}";


                ConnectToServer();
            }
            else
            {
                this.Close();
            }
        }

        private void ConnectToServer()
        {
            try
            {
                this.client = new TcpClient("127.0.0.1", 9000);
                this.stream = client.GetStream();
                this.isConnected = true;

                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                SendLoginToServer();

                RequestUserList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення до сервера: {ex.Message}");
                this.Close();
            }
        }

        private void SendLoginToServer()
        {
            try
            {
                var loginRequest = new Protocol.LoginRequest
                {
                    Username = this.username,
                    Password = ""
                };

                var message = new Protocol.NetworkMessage
                {
                    Type = Protocol.MessageType.Login,
                    Data = JsonSerializer.Serialize(loginRequest)
                };

                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка авторизації: {ex.Message}");
            }
        }

        private void RequestUserList()
        {
            try
            {
                var message = new Protocol.NetworkMessage
                {
                    Type = Protocol.MessageType.UserListRequest,
                    Data = ""
                };

                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Помилка запиту списку користувачів: {ex.Message}");
            }
        }


        private void InitializeTimer()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000;
            timer.Tick += (s, e) => RequestUserList();
            timer.Start();
        }

        private void RequestMessageHistory(string otherUser)
        {
            try
            {
                var historyRequest = new Protocol.MessageHistoryRequest
                {
                    Username1 = username,
                    Username2 = otherUser
                };

                var message = new Protocol.NetworkMessage
                {
                    Type = Protocol.MessageType.MessageHistoryRequest,
                    Data = JsonSerializer.Serialize(historyRequest)
                };

                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запиту історії повідомлень: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[4096];

            while (isConnected && client != null && client.Connected)
            {
                try
                {
                    if (stream == null) break;

                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var networkMessage = JsonSerializer.Deserialize<Protocol.NetworkMessage>(jsonData);

                    switch (networkMessage.Type)
                    {
                        case Protocol.MessageType.UserList:
                            var userListResponse = JsonSerializer.Deserialize<Protocol.UserListResponse>(networkMessage.Data);
                            UpdateUserList(userListResponse.Users);
                            break;

                        case Protocol.MessageType.PrivateMessage:
                            var privateMessage = JsonSerializer.Deserialize<Protocol.ChatMessage>(networkMessage.Data);
                            HandlePrivateMessage(privateMessage);
                            break;

                        case Protocol.MessageType.MessageHistory:
                            var historyResponse = JsonSerializer.Deserialize<Protocol.MessageHistoryResponse>(networkMessage.Data);
                            DisplayMessageHistory(historyResponse.Messages);
                            break;

                        case Protocol.MessageType.Chat:
                            break;

                        default:
                            RequestUserList();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (isConnected)
                    {
                        Console.WriteLine($"Receive error: {ex.Message}");
                    }
                    break;
                }
            }

            isConnected = false;
        }

        private void UpdateUserList(List<string> users)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UpdateUserList(users)));
                return;
            }

            listBoxUsers.Items.Clear();

            foreach (string user in users)
            {
                if (user != username)
                {
                    listBoxUsers.Items.Add(user);
                }
            }

            if (lblLoggedIn != null)
            {
                lblLoggedIn.Text = $"Користувач: {username}\nУсього користувачів на сервері: {users.Count}\nУ списку: {listBoxUsers.Items.Count}";
            }
        }


        private void UpdateUserWithNotification(string username)
        {
            try
            {
                for (int i = 0; i < listBoxUsers.Items.Count; i++)
                {
                    string item = listBoxUsers.Items[i].ToString();
                    string cleanUsername = item.Replace(" ●", "").Trim();

                    if (cleanUsername == username && !item.Contains("●"))
                    {
                        listBoxUsers.Items[i] = $"{username} ●";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка оновлення сповіщення: {ex.Message}");
            }
        }

        private void DisplayMessageHistory(List<Protocol.ChatMessage> messages)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => DisplayMessageHistory(messages)));
                return;
            }

            try
            {
                if (selectedUser != null)
                {
                    messageHistory[selectedUser] = messages;

                    listBoxMessages.Items.Clear();
                    foreach (var message in messages)
                    {
                        DisplayMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відображення історії: {ex.Message}");
            }
        }

        private void DisplayMessage(Protocol.ChatMessage message)
        {
            string displayText;
            if (message.Username == username)
            {
                displayText = $"Ви ({message.Timestamp}): {message.Content}";
            }
            else
            {
                displayText = $"{message.Username} ({message.Timestamp}): {message.Content}";
            }

            listBoxMessages.Items.Add(displayText);

            if (listBoxMessages.Items.Count > 0)
            {
                listBoxMessages.SelectedIndex = listBoxMessages.Items.Count - 1;
                listBoxMessages.SelectedIndex = -1;
            }
        }

        private void listBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxUsers.SelectedItem != null)
                {
                    selectedUser = listBoxUsers.SelectedItem.ToString().Replace(" ●", "").Trim();

                    lblChatWith.Text = $"Чат з: {selectedUser}";
                    btnSend.Enabled = true;
                    txtMessage.Enabled = true;

                    listBoxMessages.Items.Clear();

                    if (messageHistory.ContainsKey(selectedUser))
                    {
                        foreach (var message in messageHistory[selectedUser])
                        {
                            DisplayMessage(message);
                        }
                    }
                    else
                    {
                        RequestMessageHistory(selectedUser);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при виборі користувача: {ex.Message}");
            }
        }

        private void HandlePrivateMessage(Protocol.ChatMessage message)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => HandlePrivateMessage(message)));
                return;
            }

            try
            {
                string otherUser = message.Username == username ? message.Recipient : message.Username;

                if (!messageHistory.ContainsKey(otherUser))
                    messageHistory[otherUser] = new List<Protocol.ChatMessage>();

                messageHistory[otherUser].Add(message);


                if (selectedUser == otherUser)
                {
                    DisplayMessage(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка обробки повідомлення: {ex.Message}");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtMessage.Text.Trim()))
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendMessage()
        {
            if (stream == null || !isConnected || client == null || !client.Connected || selectedUser == null)
            {
                MessageBox.Show("Немає підключення до сервера або не обрано одержувача");
                return;
            }

            string text = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var chatMessage = new Protocol.ChatMessage
            {
                Username = username,
                Content = text,
                Timestamp = DateTime.Now.ToString("HH:mm:ss"),
                Recipient = selectedUser
            };

            var message = new Protocol.NetworkMessage
            {
                Type = Protocol.MessageType.PrivateMessage,
                Data = JsonSerializer.Serialize(chatMessage)
            };

            try
            {
                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(data, 0, data.Length);

                if (!messageHistory.ContainsKey(selectedUser))
                    messageHistory[selectedUser] = new List<Protocol.ChatMessage>();

                messageHistory[selectedUser].Add(chatMessage);

                DisplayMessage(chatMessage);

                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка надсилання повідомлення: {ex.Message}");
                isConnected = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            isConnected = false;

            try
            {
                if (stream != null && client != null && client.Connected)
                {
                    var message = new Protocol.NetworkMessage
                    {
                        Type = Protocol.MessageType.Disconnect,
                        Data = ""
                    };

                    string jsonData = JsonSerializer.Serialize(message);
                    byte[] data = Encoding.UTF8.GetBytes(jsonData);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch { }

            receiveThread?.Join(1000);
            stream?.Close();
            client?.Close();
        }

        public void SetUsername(string user)
        {
            username = user;
            if (lblLoggedIn != null)
                lblLoggedIn.Text = $"Користувач: {user}";
        }
    }
}