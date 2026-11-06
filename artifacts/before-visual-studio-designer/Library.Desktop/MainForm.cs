using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop;
public partial class MainForm : Form
{
    public MainForm() { InitializeComponent(); }
    public MainForm(LibraryApiClient api) : this()
    {
        welcomeLabel.Text = $"Xin chào, {api.CurrentUser!.FullName} · {api.CurrentUser.Role}";
        circulationControl.Connect(api); catalogControl.Connect(api); copiesControl.Connect(api); accountsControl.Connect(api); historyControl.Connect(api); metadataControl.Connect(api);
        if (api.CurrentUser.Role == Roles.Manager) policyControl.Connect(api); else tabs.TabPages.Remove(policyTab);
    }
}
