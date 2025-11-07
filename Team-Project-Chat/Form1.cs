using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Team_Project_Chat
{
    public partial class Form1 : Form
    {
        private TcpClient client;         // Клієнтський об’єкт TCP
        private NetworkStream stream;     // Потік для обміну даними
        private Thread receiveThread;     // Потік для отримання повідомлень
        private string username = "Unknown"; // Ім’я користувача


        // onClose event
        public Form1()
        {
            InitializeComponent();

            // Прив’язуємо подію завантаження форми
            this.Load += ClientForm_Load;
        }

        // Підключення до сервера
        private void ConnectToServer()
        {
            try
            {
                // Підключаємося до сервера (на цьому ж ПК)
                client = new TcpClient("127.0.0.1", 9000);
                stream = client.GetStream();

                // Запускаємо окремий потік для отримання повідомлень
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                AddMessage("Connected to the server");
            }
            catch
            {
                AddMessage("Unable to connect to the server");
            }
        }

        // Отримання повідомлень від сервера
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

        // Додавання повідомлення у ListBox
        private void AddMessage(string msg)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => listBox1.Items.Add(msg)));
            else
                listBox1.Items.Add(msg);
        }

        // Кнопка "Send" — відправка повідомлення на сервер
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (stream == null) return;
            string text = txtMessage.Text.Trim();
            if (text == "") return;

            string message = $"{username}: {text}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            AddMessage($"You: {text}");
            txtMessage.Clear();
        }

        // Подія при завантаженні форми — автоматичне підключення
        private void ClientForm_Load(object sender, EventArgs e)
        {
            ConnectToServer();
        }

        // Метод для встановлення імені користувача
        public void SetUsername(string user)
        {
            username = user;
            lblLoggedIn.Text = $"User: {user}";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // certain user has left the chat
        }
    }
}
