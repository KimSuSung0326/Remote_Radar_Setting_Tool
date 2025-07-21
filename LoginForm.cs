using System;
using System.Windows.Forms;


namespace radar_settinf_tool_project
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;

        public LoginForm()
        {
            // InitializeComponent() 대신 직접 초기화
            this.Text = "Login";
            this.Width = 300;
            this.Height = 180;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblUsername = new Label() { Text = "Username:", Left = 10, Top = 20, Width = 70 };
            txtUsername = new TextBox() { Left = 90, Top = 20, Width = 150 };

            Label lblPassword = new Label() { Text = "Password:", Left = 10, Top = 60, Width = 70 };
            txtPassword = new TextBox() { Left = 90, Top = 60, Width = 150, PasswordChar = '*' };

            btnLogin = new Button() { Text = "Login", Left = 90, Top = 100, Width = 80 };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "a2uict" && password == "a2uict6909")
            {
                MessageBox.Show("로그인 성공!");
                this.Hide();  
                MainForm mainForm = new MainForm();
                mainForm.ShowDialog();        // 메인 폼 열기 (모달)
                this.Close();                 // 메인폼 닫히면 로그인 폼 종료
               

                // MainForm이 없다면 이 부분 삭제하거나 주석 처리하세요.
                // var mainForm = new MainForm();
                // mainForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 틀렸습니다.");
            }
        }
    }
}
