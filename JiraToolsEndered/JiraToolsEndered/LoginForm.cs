
namespace JiraToolsEndered
{
    public partial class LoginForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;

        public string Email { get; private set; }
        public string Password { get; private set; }
        public LoginForm()
        {
            InitializeComponent();

            Text = "Login";

            Label lblEmail = new Label() { Text = "Email:", Top = 20, Left = 20 };
            txtEmail = new TextBox() { Top = 45, Left = 20, Width = 200 };

            Label lblPassword = new Label() { Text = "Password:", Top = 80, Left = 20 };
            txtPassword = new TextBox() { Top = 105, Left = 20, Width = 200, PasswordChar = '*' };

            btnLogin = new Button() { Text = "Login", Top = 140, Left = 20 };
            btnLogin.Click += BtnLogin_Click;

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            Email = txtEmail.Text;
            Password = txtPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
