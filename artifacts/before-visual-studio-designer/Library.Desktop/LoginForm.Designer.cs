#nullable enable
namespace Library.Desktop;
partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private Label heading = null!;
    private Label subtitle = null!;
    private Label urlLabel = null!;
    private TextBox urlBox = null!;
    private Label userLabel = null!;
    private TextBox userBox = null!;
    private Label passwordLabel = null!;
    private TextBox passwordBox = null!;
    private Button loginButton = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        heading = new Label();
        heading.Name = "heading";
        heading.Location = new Point(40, 35);
        heading.Size = new Size(600, 27);
        heading.Text = "THƯ VIỆN ĐẠI HỌC · QUẦY THỦ THƯ";
        heading.AutoEllipsis = true;
        
        subtitle = new Label();
        subtitle.Name = "subtitle";
        subtitle.Location = new Point(40, 85);
        subtitle.Size = new Size(600, 27);
        subtitle.Text = "Đăng nhập để bắt đầu phiên làm việc";
        subtitle.AutoEllipsis = true;
        
        urlLabel = new Label();
        urlLabel.Name = "urlLabel";
        urlLabel.Location = new Point(40, 140);
        urlLabel.Size = new Size(300, 27);
        urlLabel.Text = "Địa chỉ Server";
        urlLabel.AutoEllipsis = true;
        
        urlBox = new TextBox();
        urlBox.Name = "urlBox";
        urlBox.Location = new Point(40, 170);
        urlBox.Size = new Size(650, 31);
        urlBox.Text = "http://localhost:5080";
        urlBox.TextChanged += urlBox_TextChanged;
        
        userLabel = new Label();
        userLabel.Name = "userLabel";
        userLabel.Location = new Point(40, 230);
        userLabel.Size = new Size(300, 27);
        userLabel.Text = "Tài khoản";
        userLabel.AutoEllipsis = true;
        
        userBox = new TextBox();
        userBox.Name = "userBox";
        userBox.Location = new Point(40, 260);
        userBox.Size = new Size(650, 31);
        userBox.TextChanged += userBox_TextChanged;
        
        passwordLabel = new Label();
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Location = new Point(40, 320);
        passwordLabel.Size = new Size(300, 27);
        passwordLabel.Text = "Mật khẩu";
        passwordLabel.AutoEllipsis = true;
        
        passwordBox = new TextBox();
        passwordBox.Name = "passwordBox";
        passwordBox.Location = new Point(40, 350);
        passwordBox.Size = new Size(650, 31);
        passwordBox.UseSystemPasswordChar = true;
        passwordBox.TextChanged += passwordBox_TextChanged;
        loginButton = new Button();
        loginButton.Name = "loginButton";
        loginButton.Location = new Point(40, 420);
        loginButton.Size = new Size(220, 38);
        loginButton.Text = "Đăng nhập →";
        loginButton.BackColor = Color.FromArgb(48, 78, 56); loginButton.ForeColor = Color.White; loginButton.FlatStyle = FlatStyle.Flat; loginButton.FlatAppearance.BorderSize = 0;
        loginButton.Enabled = false;
        loginButton.Click += loginButton_Click;
        statusLabel = new Label();
        statusLabel.Name = "statusLabel";
        statusLabel.Location = new Point(40, 485);
        statusLabel.Size = new Size(700, 27);
        statusLabel.Text = "Dùng tài khoản Librarian hoặc Manager được cấp.";
        statusLabel.AutoEllipsis = true;
        

        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(248, 248, 242);
        ForeColor = Color.FromArgb(36, 56, 47);
        ClientSize = new Size(900, 720);
        Name = "LoginForm";
        AcceptButton = loginButton;
        Load += LoginForm_Load;
        Text = "Thư viện Đại học";
        StartPosition = FormStartPosition.CenterScreen; MinimumSize = new Size(900, 760);
        Controls.Add(heading);
        Controls.Add(subtitle);
        Controls.Add(urlLabel);
        Controls.Add(urlBox);
        Controls.Add(userLabel);
        Controls.Add(userBox);
        Controls.Add(passwordLabel);
        Controls.Add(passwordBox);
        Controls.Add(loginButton);
        Controls.Add(statusLabel);

        ResumeLayout(false);
        PerformLayout();
    }
}
