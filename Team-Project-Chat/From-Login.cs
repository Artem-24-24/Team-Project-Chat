using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Team_Project_Chat
{
    public partial class From_Login : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private string loggedInUsername;

        public From_Login()
        {
            InitializeComponent();
            this.AcceptButton = btn_login;
            ClearErrorMessage();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = tb_login.Text.Trim();
            string password = tb_password.Text.Trim();

            ClearErrorMessage();

            if (!ValidateInput(username, password))
                return;

            if (ConnectToServer())
            {
                if (AttemptLogin(username, password))
                {
                    loggedInUsername = username;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    CloseConnection();
                }
            }
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            string username = tb_login.Text.Trim();
            string password = tb_password.Text.Trim();

            ClearErrorMessage();

            if (!ValidateInput(username, password))
                return;

            if (ConnectToServer())
            {
                if (AttemptRegister(username, password))
                {
                    loggedInUsername = username;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    CloseConnection();
                }
            }
        }

        private bool ValidateInput(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Введіть ім'я користувача і пароль");
                return false;
            }

            if (username.Length < 3)
            {
                ShowError("Ім'я користувача має бути не менше 3-х символів");
                return false;
            }

            if (password.Length < 3)
            {
                ShowError("Пароль має бути не менше 3-х символів");
                return false;
            }

            return true;
        }

        private bool ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 9000);
                stream = client.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Не вдалося підключитися до сервера: {ex.Message}");
                return false;
            }
        }

        private bool AttemptLogin(string username, string password)
        {
            var loginRequest = new Protocol.LoginRequest
            {
                Username = username,
                Password = password
            };

            var message = new Protocol.NetworkMessage
            {
                Type = Protocol.MessageType.Login,
                Data = JsonSerializer.Serialize(loginRequest)
            };

            return SendAndWaitForResponse(message, true);
        }

        private bool AttemptRegister(string username, string password)
        {
            var registerRequest = new Protocol.LoginRequest
            {
                Username = username,
                Password = password
            };

            var message = new Protocol.NetworkMessage
            {
                Type = Protocol.MessageType.Register,
                Data = JsonSerializer.Serialize(registerRequest)
            };

            return SendAndWaitForResponse(message, false);
        }

        private bool SendAndWaitForResponse(Protocol.NetworkMessage message, bool isLogin)
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                var responseMessage = JsonSerializer.Deserialize<Protocol.NetworkMessage>(responseJson);
                var loginResponse = JsonSerializer.Deserialize<Protocol.LoginResponse>(responseMessage.Data);

                if (loginResponse.Success)
                {
                    return true;
                }
                else
                {
                    if (isLogin && loginResponse.Message.Contains("облікові дані") || loginResponse.Message.Contains("credentials"))
                    {
                        ShowError("Неправильне ім'я користувача або пароль");
                    }
                    else if (isLogin && loginResponse.Message.Contains("вже ввійшов") || loginResponse.Message.Contains("already logged"))
                    {
                        ShowError("Користувач авторизований");
                    }
                    else if (!isLogin && loginResponse.Message.Contains("існує") || loginResponse.Message.Contains("exists"))
                    {
                        ShowError("Користувач з таким іменем вже існує");
                    }
                    else
                    {
                        ShowError(loginResponse.Message);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка зв'язку с сервером: {ex.Message}");
                return false;
            }
        }

        private void ShowError(string message)
        {
            lbl_error.Text = message;
        }

        private void ClearErrorMessage()
        {
            lbl_error.Text = "";
        }

        private void CloseConnection()
        {
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch { }
            finally
            {
                stream = null;
                client = null;
            }
        }

        public TcpClient GetClient()
        {
            return client;
        }

        public NetworkStream GetStream()
        {
            return stream;
        }

        public string GetUsername()
        {
            return loggedInUsername ?? tb_login.Text.Trim();
        }

        private void tb_password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btn_login_Click(sender, e);
            }
        }
    }
}