using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace Team_Project_Chat
{
    public partial class Form1 : Form
    {
        private TcpClient client;         // �볺������� �ᒺ�� TCP
        private NetworkStream stream;     // ���� ��� ����� ������
        private Thread receiveThread;     // ���� ��� ��������� ����������
        private string username = "Unknown"; // ��� �����������
        private int userId = -1;          // ID користувача
        private string password = "";     // Password for authentication
        private string authType = "login"; // Auth type (login or register)
        private bool isAuthenticated = false; // Authentication status


        // onClose event
        public Form1()
        {
            InitializeComponent();

            // ��������� ���� ������������ �����
            this.Load += ClientForm_Load;
        }

        // ϳ��������� �� �������
        private void ConnectToServer()
        {
            try
            {
                // ϳ���������� �� ������� (�� ����� � ��)
                client = new TcpClient("127.0.0.1", 9000);
                stream = client.GetStream();

                // ��������� ������� ���� ��� ��������� ����������
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                AddMessage("Connected to the server");
                
                // Re-authenticate with the server for this persistent connection
                if (userId > 0)
                {
                    SendAuthenticationForChat();
                }
            }
            catch (Exception ex)
            {
                AddMessage($"Unable to connect to the server: {ex.Message}");
            }
        }

        private void SendAuthenticationForChat()
        {
            try
            {
                // Send authentication request to server
                var authMessage = new ProtocolMessage
                {
                    Type = authType,
                    Username = username,
                    Password = password
                };

                string json = JsonSerializer.Serialize(authMessage);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);

                AddMessage($"Authenticating as {username}...");

                // The response will be received in ReceiveMessages thread
            }
            catch (Exception ex)
            {
                AddMessage($"Authentication error: {ex.Message}");
            }
        }

        // ��������� ���������� �� �������
        private void ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    // Try to parse as protocol message
                    try
                    {
                        var protocolMsg = JsonSerializer.Deserialize<ProtocolMessage>(msg);
                        
                        if (protocolMsg != null && protocolMsg.Type == "response")
                        {
                            // This is an authentication response
                            if (protocolMsg.Success)
                            {
                                userId = protocolMsg.UserId;
                                isAuthenticated = true;
                                AddMessage($"✓ Authentication successful! User ID: {userId}");
                                AddMessage("You can now send messages.");
                            }
                            else
                            {
                                AddMessage($"✗ Authentication failed: {protocolMsg.ErrorMessage}");
                                AddMessage("Please restart and try again.");
                            }
                        }
                        else
                        {
                            // Not a protocol message or different type, treat as chat
                            AddMessage(msg);
                        }
                    }
                    catch
                    {
                        // If not JSON, treat as regular chat message
                        AddMessage(msg);
                    }
                }
                catch
                {
                    AddMessage("Disconnected");
                    break;
                }
            }
        }

        // ��������� ����������� � ListBox
        private void AddMessage(string msg)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => listBox1.Items.Add(msg)));
            else
                listBox1.Items.Add(msg);
        }

        // ������ "Send" � �������� ����������� �� ������
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (stream == null) return;
            
            if (!isAuthenticated)
            {
                AddMessage("Please wait for authentication to complete.");
                return;
            }
            
            string text = txtMessage.Text.Trim();
            if (text == "") return;

            try
            {
                // Send message using protocol
                var chatMessage = new ProtocolMessage
                {
                    Type = "chat",
                    Content = text,
                    Username = username
                };

                string json = JsonSerializer.Serialize(chatMessage);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);

                AddMessage($"You: {text}");
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                AddMessage($"Error sending message: {ex.Message}");
            }
        }

        // ���� ��� ������������ ����� � ����������� ����������
        private void ClientForm_Load(object sender, EventArgs e)
        {
            ConnectToServer();
        }

        // ����� ��� ������������ ����� �����������
        public void SetUsername(string user)
        {
            username = user;
            lblLoggedIn.Text = $"User: {user}";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // certain user has left the chat
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            From_Login loginForm = new From_Login();
            var result = loginForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.username = loginForm.Username;
                this.password = loginForm.Password;
                this.authType = loginForm.AuthType;
                this.Text = "Chat Client - " + loginForm.Username;
                lblLoggedIn.Text = $"User: {loginForm.Username}";
            }
            else
            {
                this.Close();
            }
        }
    }
}
