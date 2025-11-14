using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Team_Project_Chat
{
    public partial class From_Login : Form
    {
        private string loggedInUsername;

        public From_Login()
        {
            InitializeComponent();
            this.AcceptButton = btn_login;
            ClearErrorMessage();
        }

        public string GetUsername()
        {
            return loggedInUsername;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = tb_login.Text.Trim();
            string password = tb_password.Text.Trim();

            ClearErrorMessage();

            if (!ValidateInput(username, password))
                return;

            try
            {
                using (TcpClient tempClient = new TcpClient())
                {
                    tempClient.Connect("127.0.0.1", 9000);
                    NetworkStream tempStream = tempClient.GetStream();

                    if (AttemptLogin(username, password, tempStream))
                    {
                        loggedInUsername = username;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Не вдалося підключитися до сервера: {ex.Message}");
            }
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            string username = tb_login.Text.Trim();
            string password = tb_password.Text.Trim();

            ClearErrorMessage();

            if (!ValidateInput(username, password))
                return;

            try
            {
                Console.WriteLine("Створюю TCP клієнт...");
                using (TcpClient tempClient = new TcpClient())
                {
                    Console.WriteLine("Підключаюся до 127.0.0.1:9000...");
                    tempClient.Connect("127.0.0.1", 9000);
                    Console.WriteLine("Підключено, отримую потік...");
                    NetworkStream tempStream = tempClient.GetStream();

                    if (AttemptRegister(username, password, tempStream))
                    {
                        Console.WriteLine("Реєстрація успішна, закриваю форму...");
                        loggedInUsername = username;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        Console.WriteLine("Реєстрація невдала");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка підключення: {ex.Message}");
                ShowError($"Не вдалося підключитися до сервера: {ex.Message}");
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

        private bool AttemptLogin(string username, string password, NetworkStream stream)
        {
            try
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

                return SendAndWaitForResponse(message, true, stream);
            }
            catch (Exception ex)
            {
                ShowError($"Помилка при вході: {ex.Message}");
                return false;
            }
        }

        private bool AttemptRegister(string username, string password, NetworkStream stream)
        {
            try
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

                return SendAndWaitForResponse(message, false, stream);
            }
            catch (Exception ex)
            {
                ShowError($"Помилка при реєстрації: {ex.Message}");
                return false;
            }
        }

        private bool SendAndWaitForResponse(Protocol.NetworkMessage message, bool isLogin, NetworkStream stream)
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[4096];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        ShowError("Сервер не відповідає");
                        return false;
                    }

                    string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var responseMessage = JsonSerializer.Deserialize<Protocol.NetworkMessage>(responseJson);

                    if (responseMessage == null)
                    {
                        ShowError("Некоректна відповідь від сервера");
                        return false;
                    }


                    if ((isLogin && responseMessage.Type == Protocol.MessageType.Login) ||
                        (!isLogin && responseMessage.Type == Protocol.MessageType.Register))
                    {
                        var loginResponse = JsonSerializer.Deserialize<Protocol.LoginResponse>(responseMessage.Data);

                        if (loginResponse == null)
                        {
                            ShowError("Некоректні дані відповіді");
                            return false;
                        }

                        if (loginResponse.Success)
                        {
                            return true;
                        }
                        else
                        {
                            string errorMessage = loginResponse.Message ?? "Невідома помилка";
                            ShowError(errorMessage);
                            return false;
                        }
                    }

                    else if (responseMessage.Type == Protocol.MessageType.UserList)
                    {

                        continue;
                    }
                    else
                    {
                        ShowError($"Неочікувана відповідь від сервера: {responseMessage.Type}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Помилка зв'язку з сервером: {ex.Message}");
                return false;
            }
        }

        private void ShowError(string message)
        {
            if (lbl_error.InvokeRequired)
            {
                lbl_error.Invoke(new Action(() => lbl_error.Text = message));
            }
            else
            {
                lbl_error.Text = message;
            }
        }

        private void ClearErrorMessage()
        {
            if (lbl_error.InvokeRequired)
            {
                lbl_error.Invoke(new Action(() => lbl_error.Text = ""));
            }
            else
            {
                lbl_error.Text = "";
            }
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