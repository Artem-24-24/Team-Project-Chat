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
            }
            catch
            {
                AddMessage("Unable to connect to the server");
            }
        }

        // ��������� ���������� �� �������
        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    AddMessage(msg);
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
                this.userId = loginForm.UserId;
                this.SetUsername(loginForm.Username);
                this.Text = "Chat Client - " + loginForm.Username;
            }
            else
            {
                this.Close();
            }
        }
    }
}
