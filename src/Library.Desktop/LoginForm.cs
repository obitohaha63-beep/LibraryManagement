using Library.Desktop.Services;

namespace Library.Desktop;

public partial class LoginForm : Form
{
    private readonly LibraryApiClient? api;
    private bool isSigningIn;

    // Visual Studio Designer uses this constructor; no API call is made here.
    public LoginForm()
    {
        InitializeComponent();
    }

    public LoginForm(LibraryApiClient api) : this()
    {
        this.api = api;
    }

    // Double-click the form background in Designer to open this Load event.
    private void LoginForm_Load(object? sender, EventArgs e)
    {
        UpdateInputState();
        userBox.Select();
    }

    // Double-click the username TextBox in Designer to open this event.
    private void userBox_TextChanged(object? sender, EventArgs e)
    {
        UpdateInputState();
    }

    private void passwordBox_TextChanged(object? sender, EventArgs e)
    {
        UpdateInputState();
    }

    private void urlBox_TextChanged(object? sender, EventArgs e)
    {
        UpdateInputState();
    }

    private void UpdateInputState()
    {
        if (isSigningIn) return;

        if (!Uri.TryCreate(urlBox.Text.Trim(), UriKind.Absolute, out var serverUrl)
            || serverUrl.Scheme is not ("http" or "https")
            || string.IsNullOrWhiteSpace(serverUrl.Host))
        {
            loginButton.Enabled = false;
            statusLabel.Text = "Nhập địa chỉ server bắt đầu bằng http:// hoặc https://.";
        }
        else if (string.IsNullOrWhiteSpace(userBox.Text))
        {
            loginButton.Enabled = false;
            statusLabel.Text = "Vui lòng nhập tài khoản.";
        }
        else if (string.IsNullOrWhiteSpace(passwordBox.Text))
        {
            loginButton.Enabled = false;
            statusLabel.Text = "Vui lòng nhập mật khẩu.";
        }
        else
        {
            loginButton.Enabled = true;
            statusLabel.Text = "Nhấn Đăng nhập hoặc Enter để tiếp tục.";
        }
    }

    // Only this Click event starts authentication; typing never sends credentials.
    private async void loginButton_Click(object? sender, EventArgs e)
    {
        if (isSigningIn) return;
        UpdateInputState();
        if (!loginButton.Enabled) return;
        if (api == null)
        {
            statusLabel.Text = "Hãy chạy ứng dụng từ Library.Desktop để kết nối server.";
            return;
        }

        isSigningIn = true;
        loginButton.Enabled = false;
        urlBox.Enabled = false;
        userBox.Enabled = false;
        passwordBox.Enabled = false;
        statusLabel.Text = "Đang đăng nhập…";

        try
        {
            api.SetBaseUrl(urlBox.Text.Trim());
            await api.LoginAsync(userBox.Text.Trim(), passwordBox.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            statusLabel.Text = ex is HttpRequestException
                ? "Không kết nối được server. Kiểm tra địa chỉ và khởi động Server."
                : ex.Message;
        }
        finally
        {
            isSigningIn = false;
            if (!IsDisposed)
            {
                urlBox.Enabled = true;
                userBox.Enabled = true;
                passwordBox.Enabled = true;
                loginButton.Enabled = true;
            }
        }
    }
}
