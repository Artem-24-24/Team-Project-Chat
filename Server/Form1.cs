using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
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

    public partial class Form1 : Form
    {
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

            Log("Тест API завершено");

            return Task.CompletedTask;
        }

        #endregion

        private void txtPort_TextChanged(object sender, EventArgs e)
        {
        }
    }
}