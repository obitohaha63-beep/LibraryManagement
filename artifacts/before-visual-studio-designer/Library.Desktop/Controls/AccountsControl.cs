using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class AccountsControl : UserControl
{
    private LibraryApiClient? api; private int page = 1; private string? selectedId;
    public AccountsControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) { api = client; roleCombo.Items.Add(Roles.Reader); if (client.CurrentUser!.Role == Roles.Manager) roleCombo.Items.AddRange([Roles.Librarian, Roles.Manager]); roleCombo.SelectedIndex = 0; filterRoleCombo.Items.AddRange(roleCombo.Items.Cast<object>().ToArray()); filterRoleCombo.SelectedIndex = 0; }
    private async Task LoadAsync() { var rows = await api!.GetAsync<Page<UserDto>>($"api/users?q={Uri.EscapeDataString(searchBox.Text)}&role={filterRoleCombo.Text}&page={page}"); grid.DataSource = rows.Items; pageLabel.Text = $"Trang {page} · {rows.Total} tài khoản"; prevButton.Enabled = page > 1; nextButton.Enabled = page * rows.Size < rows.Total; }
    private async void Search_Click(object? s, EventArgs e) { page = 1; await Ui.RunAsync(searchButton, LoadAsync, statusLabel); }
    private async void Prev_Click(object? s, EventArgs e) { page = Math.Max(1, page - 1); await Ui.RunAsync(prevButton, LoadAsync, statusLabel); }
    private async void Next_Click(object? s, EventArgs e) { page++; await Ui.RunAsync(nextButton, LoadAsync, statusLabel); }
    private void Select_Row(object? s, EventArgs e) { if (grid.CurrentRow?.DataBoundItem is not UserDto u) return; selectedId = u.Id; userBox.Text = u.UserName; userBox.ReadOnly = true; nameBox.Text = u.FullName; activeCheck.Checked = u.Active; roleCombo.SelectedItem = u.Role; passwordBox.Enabled = false; }
    private void New_Click(object? s, EventArgs e) { selectedId = null; userBox.ReadOnly = false; passwordBox.Enabled = true; userBox.Clear(); nameBox.Clear(); passwordBox.Clear(); activeCheck.Checked = true; }
    private async void Save_Click(object? s, EventArgs e) => await Ui.RunAsync(saveButton, async () => {
        if (selectedId != null) await api!.SaveAsync($"api/users/{selectedId}", new UserUpdate(nameBox.Text, roleCombo.Text, activeCheck.Checked), true);
        else await api!.SaveAsync("api/users", new UserRequest(userBox.Text, nameBox.Text, passwordBox.Text, roleCombo.Text, activeCheck.Checked)); await LoadAsync(); }, statusLabel);
}
