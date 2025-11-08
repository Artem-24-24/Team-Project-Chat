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
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AuthType { get; set; }
        public string Password { get; set; }
        
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

            TryAuthenticate("login", tb_login.Text, tb_password.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_login.Text) || string.IsNullOrWhiteSpace(tb_password.Text))
            {
                MessageBox.Show("Будь ласка, введіть логін і пароль", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TryAuthenticate("register", tb_login.Text, tb_password.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool TryAuthenticate(string type, string username, string password)
        {
            // For now, just validate that fields are not empty
            // The actual authentication will happen when connecting to chat
            // This allows us to maintain a single persistent connection
            
            // Store the auth type for later use by Form1
            this.AuthType = type;
            this.Username = username;
            this.Password = password;
            
            // Always return true here - actual validation happens on server when chat connects
            return true;
        }

        public string AuthType { get; private set; }
        public string Password { get; private set; }
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
