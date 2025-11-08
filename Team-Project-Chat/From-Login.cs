using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Team_Project_Chat
{
    public partial class From_Login : Form
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        
        public From_Login()
        {
            InitializeComponent();
            this.AcceptButton = btn_login;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void From_Login_Load(object sender, EventArgs e)
        {

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_login.Text) || string.IsNullOrWhiteSpace(tb_password.Text))
            {
                MessageBox.Show("Будь ласка, введіть логін і пароль", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TryAuthenticate("login", tb_login.Text, tb_password.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_login.Text) || string.IsNullOrWhiteSpace(tb_password.Text))
            {
                MessageBox.Show("Будь ласка, введіть логін і пароль", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TryAuthenticate("register", tb_login.Text, tb_password.Text))
            {
                MessageBox.Show("Реєстрація успішна! Ви увійшли в систему.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Користувач вже існує або помилка реєстрації", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryAuthenticate(string type, string username, string password)
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 9000))
                using (NetworkStream stream = client.GetStream())
                {
                    // Send authentication request
                    var authMessage = new ProtocolMessage
                    {
                        Type = type,
                        Username = username,
                        Password = password
                    };

                    string json = JsonSerializer.Serialize(authMessage);
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    stream.Write(data, 0, data.Length);

                    // Wait for response
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    var responseMsg = JsonSerializer.Deserialize<ProtocolMessage>(response);

                    if (responseMsg != null && responseMsg.Success)
                    {
                        UserId = responseMsg.UserId;
                        Username = responseMsg.Username;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення до сервера: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
    }

    // Protocol message class - must match server
    public class ProtocolMessage
    {
        public string Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Content { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int UserId { get; set; }
    }
}
