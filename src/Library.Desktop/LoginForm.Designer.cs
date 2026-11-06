#nullable enable
namespace Library.Desktop;
partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
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
        heading = new Label();
        subtitle = new Label();
        urlLabel = new Label();
        urlBox = new TextBox();
        userLabel = new Label();
        userBox = new TextBox();
        passwordLabel = new Label();
        passwordBox = new TextBox();
        loginButton = new Button();
        statusLabel = new Label();
        SuspendLayout();
        // 
        // heading
        // 
        heading.AutoEllipsis = true;
        heading.Location = new Point(60, 55);
        heading.Name = "heading";
        heading.Size = new Size(600, 27);
        heading.TabIndex = 0;
        heading.Text = "THƯ VIỆN ĐẠI HỌC · QUẦY THỦ THƯ";
        // 
        // subtitle
        // 
        subtitle.AutoEllipsis = true;
        subtitle.Location = new Point(60, 105);
        subtitle.Name = "subtitle";
        subtitle.Size = new Size(600, 27);
        subtitle.TabIndex = 1;
        subtitle.Text = "Đăng nhập để bắt đầu phiên làm việc";
        // 
        // urlLabel
        // 
        urlLabel.AutoEllipsis = true;
        urlLabel.Location = new Point(60, 160);
        urlLabel.Name = "urlLabel";
        urlLabel.Size = new Size(300, 27);
        urlLabel.TabIndex = 2;
        urlLabel.Text = "Địa chỉ Server";
        // 
        // urlBox
        // 
        urlBox.Location = new Point(60, 190);
        urlBox.Name = "urlBox";
        urlBox.Size = new Size(650, 30);
        urlBox.TabIndex = 3;
        urlBox.Text = "http://localhost:5080";
        urlBox.TextChanged += urlBox_TextChanged;
        // 
        // userLabel
        // 
        userLabel.AutoEllipsis = true;
        userLabel.Location = new Point(60, 250);
        userLabel.Name = "userLabel";
        userLabel.Size = new Size(300, 27);
        userLabel.TabIndex = 4;
        userLabel.Text = "Tài khoản";
        // 
        // userBox
        // 
        userBox.Location = new Point(60, 280);
        userBox.Name = "userBox";
        userBox.Size = new Size(650, 30);
        userBox.TabIndex = 5;
        userBox.TextChanged += userBox_TextChanged;
        // 
        // passwordLabel
        // 
        passwordLabel.AutoEllipsis = true;
        passwordLabel.Location = new Point(60, 340);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(300, 27);
        passwordLabel.TabIndex = 6;
        passwordLabel.Text = "Mật khẩu";
        // 
        // passwordBox
        // 
        passwordBox.Location = new Point(60, 370);
        passwordBox.Name = "passwordBox";
        passwordBox.Size = new Size(650, 30);
        passwordBox.TabIndex = 7;
        passwordBox.UseSystemPasswordChar = true;
        passwordBox.TextChanged += passwordBox_TextChanged;
        // 
        // loginButton
        // 
        loginButton.BackColor = Color.FromArgb(48, 78, 56);
        loginButton.Enabled = false;
        loginButton.FlatAppearance.BorderSize = 0;
        loginButton.FlatStyle = FlatStyle.Flat;
        loginButton.ForeColor = Color.White;
        loginButton.Location = new Point(60, 440);
        loginButton.Name = "loginButton";
        loginButton.Size = new Size(220, 38);
        loginButton.TabIndex = 8;
        loginButton.Text = "Đăng nhập →";
        loginButton.UseVisualStyleBackColor = false;
        loginButton.Click += loginButton_Click;
        // 
        // statusLabel
        // 
        statusLabel.AutoEllipsis = true;
        statusLabel.Location = new Point(60, 505);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(700, 27);
        statusLabel.TabIndex = 9;
        statusLabel.Text = "Dùng tài khoản Librarian hoặc Manager được cấp.";
        // 
        // LoginForm
        // 
        AcceptButton = loginButton;
        AutoScaleDimensions = new SizeF(9F, 23F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 248, 242);
        ClientSize = new Size(900, 720);
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
        Font = new Font("Segoe UI", 10F);
        ForeColor = Color.FromArgb(36, 56, 47);
        MinimumSize = new Size(900, 760);
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Thư viện Đại học";
        Load += LoginForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }
}
