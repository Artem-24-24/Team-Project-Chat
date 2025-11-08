namespace Team_Project_Chat
{
    partial class From_Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_login = new TextBox();
            tb_password = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btn_login = new Button();
            btn_register = new Button();
            lbl_error = new Label();
            SuspendLayout();
            // 
            // tb_login
            // 
            tb_login.Location = new Point(473, 247);
            tb_login.Name = "tb_login";
            tb_login.Size = new Size(223, 27);
            tb_login.TabIndex = 0;
            // 
            // tb_password
            // 
            tb_password.Location = new Point(473, 338);
            tb_password.Name = "tb_password";
            tb_password.PasswordChar = '*';
            tb_password.Size = new Size(223, 27);
            tb_password.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(473, 224);
            label1.Name = "label1";
            label1.Size = new Size(47, 20);
            label1.TabIndex = 2;
            label1.Text = "Логін";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(473, 315);
            label2.Name = "label2";
            label2.Size = new Size(62, 20);
            label2.TabIndex = 3;
            label2.Text = "Пароль";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label3.Location = new Point(495, 151);
            label3.Name = "label3";
            label3.Size = new Size(184, 37);
            label3.TabIndex = 4;
            label3.Text = "Авторизація";
            // 
            // btn_login
            // 
            btn_login.Location = new Point(473, 413);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(100, 60);
            btn_login.TabIndex = 2;
            btn_login.Text = "Увійти";
            btn_login.UseVisualStyleBackColor = true;
            btn_login.Click += btn_login_Click;
            // 
            // btn_register
            // 
            btn_register.Location = new Point(578, 413);
            btn_register.Name = "btn_register";
            btn_register.Size = new Size(118, 60);
            btn_register.TabIndex = 3;
            btn_register.Text = "Реєстрація";
            btn_register.UseVisualStyleBackColor = true;
            btn_register.Click += btn_register_Click;
            // 
            // lbl_error
            // 
            lbl_error.AutoSize = true;
            lbl_error.ForeColor = Color.Red;
            lbl_error.Location = new Point(473, 380);
            lbl_error.Name = "lbl_error";
            lbl_error.Size = new Size(0, 20);
            lbl_error.TabIndex = 7;
            // 
            // From_Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1163, 675);
            Controls.Add(lbl_error);
            Controls.Add(btn_register);
            Controls.Add(btn_login);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tb_password);
            Controls.Add(tb_login);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "From_Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Вход в систему";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public TextBox tb_login;
        private TextBox tb_password;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn_login;
        private Button btn_register;
        private Label lbl_error;
    }
}