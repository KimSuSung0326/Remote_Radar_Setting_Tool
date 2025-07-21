using System;
using System.Drawing;
using System.Windows.Forms;

namespace radar_settinf_tool_project
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblStatus;  // 상태 메시지 표시용 라벨

        public LoginForm()
        {
            this.Text = "Login";
            this.Width = 300;
            this.Height = 220;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblUsername = new Label() { Text = "Username:", Left = 10, Top = 20, Width = 70 };
            txtUsername = new TextBox() { Left = 90, Top = 20, Width = 150 };

            Label lblPassword = new Label() { Text = "Password:", Left = 10, Top = 60, Width = 70 };
            txtPassword = new TextBox() { Left = 90, Top = 60, Width = 150, PasswordChar = '*' };

            btnLogin = new Button() { Text = "Login", Left = 90, Top = 100, Width = 80 };
            btnLogin.Click += BtnLogin_Click;

            lblStatus = new Label()
            {
                Text = "",
                Left = 110,
                Top = 140,
                Width = 250,
                ForeColor = Color.Green,
                Visible = false
            };

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblStatus);  // 상태 라벨 추가
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "a2uict" && password == "a2uict6909")
            {
                lblStatus.Text = "로그인 성공!";
                lblStatus.Visible = true;

                Timer timer = new Timer();
                timer.Interval = 1000;
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    this.Hide();
                    MainForm mainForm = new MainForm();
                    mainForm.ShowDialog();
                    this.Close();
                };
                timer.Start();
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 틀렸습니다.");
            }
        }
    }
}
