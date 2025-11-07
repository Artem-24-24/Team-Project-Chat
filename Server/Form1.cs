using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        private TcpListener listener;             // Об’єкт, який слухає підключення клієнтів
        private Thread listenThread;              // Потік, який приймає клієнтів
        private List<TcpClient> clients = new();  // Список усіх підключених клієнтів
        private bool running = false;             // Статус роботи сервера

        public Form1()
        {
            InitializeComponent();
        }

        // Кнопка "Start" — запуск сервера
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

                Log("Server started, Port: 9000");
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

                    Log("New client connected");
                }
            }
            catch (SocketException ex)
            {
                Log($"Server was stopped: {ex.Message}");
            }
        }

        // Обробка повідомлень від конкретного клієнта
        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (running)
            {
                try
                {
                    // Зчитуємо повідомлення
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Log($"<Message>: {msg}");

                    // Відправляємо повідомлення іншим клієнтам
                    Broadcast(msg, client);
                }
                catch
                {
                    break;
                }
            }

            // Якщо клієнт відключився
            clients.Remove(client);
            UpdateClientCount();
            Log("Client disconnected");
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
        private void Log(string text)
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
}
