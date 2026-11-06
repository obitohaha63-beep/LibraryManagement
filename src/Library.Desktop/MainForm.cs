using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop;
public partial class MainForm : Form
{
    private LibraryApiClient? api;

    public MainForm() { InitializeComponent(); }
    public MainForm(LibraryApiClient api) : this()
    {
        this.api = api;
        if (api.CurrentUser!.Role != Roles.Manager) tabs.TabPages.Remove(policyTab);
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        UpdateWindowTitle();
    }

    private void tabs_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateWindowTitle();
    }

    // These events belong to the UserControl instances placed on MainForm in Designer.
    // To edit events of an input inside a UserControl, open that UserControl's own Designer.
    private void circulationControl_Load(object? sender, EventArgs e)
    {
        if (api != null) circulationControl.Connect(api);
    }
    private void catalogControl_Load(object? sender, EventArgs e)
    {
        if (api != null) catalogControl.Connect(api);
    }
    private void copiesControl_Load(object? sender, EventArgs e)
    {
        if (api != null) copiesControl.Connect(api);
    }
    private void accountsControl_Load(object? sender, EventArgs e)
    {
        if (api != null) accountsControl.Connect(api);
    }
    private void historyControl_Load(object? sender, EventArgs e)
    {
        if (api != null) historyControl.Connect(api);
    }
    private void metadataControl_Load(object? sender, EventArgs e)
    {
        if (api != null) metadataControl.Connect(api);
    }
    private void policyControl_Load(object? sender, EventArgs e)
    {
        if (api?.CurrentUser?.Role == Roles.Manager) policyControl.Connect(api);
    }

    private void UpdateWindowTitle()
    {
        Text = $"Thư viện Đại học · {tabs.SelectedTab?.Text ?? "Bàn làm việc"}";
        if (api?.CurrentUser is { } user)
            welcomeLabel.Text = $"Xin chào, {user.FullName} · {user.Role} · {tabs.SelectedTab?.Text}";
    }
}
