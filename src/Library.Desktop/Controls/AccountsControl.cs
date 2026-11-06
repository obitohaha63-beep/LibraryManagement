using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class AccountsControl : UserControl
{
    private LibraryApiClient? api;
    private int page = 1;
    private string? selectedId;
    private bool ready;
    private bool fillingForm;

    public AccountsControl() { InitializeComponent(); }

    public void Connect(LibraryApiClient client)
    {
        api = client;
        fillingForm = true;
        roleCombo.Items.Clear();
        filterRoleCombo.Items.Clear();
        roleCombo.Items.Add(Roles.Reader);
        if (client.CurrentUser!.Role == Roles.Manager) roleCombo.Items.AddRange([Roles.Librarian, Roles.Manager]);
        roleCombo.SelectedIndex = 0;
        filterRoleCombo.Items.AddRange(roleCombo.Items.Cast<object>().ToArray());
        filterRoleCombo.SelectedIndex = 0;
        fillingForm = false;
    }

    private void AccountsControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        prevButton.Enabled = false;
        nextButton.Enabled = false;
        statusLabel.Text = "Chọn vai trò cần tìm, hoặc bấm Tạo tài khoản để nhập mới.";
    }

    private async Task LoadAsync()
    {
        var rows = await Ui.RequireApi(api).GetAsync<Page<UserDto>>($"api/users?q={Uri.EscapeDataString(searchBox.Text)}&role={filterRoleCombo.Text}&page={page}");
        grid.DataSource = rows.Items;
        pageLabel.Text = $"Trang {page} · {rows.Total} tài khoản";
        prevButton.Enabled = page > 1;
        nextButton.Enabled = page * rows.Size < rows.Total;
    }
    private async void searchButton_Click(object? sender, EventArgs e)
    {
        page = 1;
        await Ui.RunAsync(this, LoadAsync, statusLabel);
    }
    private async void prevButton_Click(object? sender, EventArgs e)
    {
        page = Math.Max(1, page - 1);
        await Ui.RunAsync(this, LoadAsync, statusLabel);
    }
    private async void nextButton_Click(object? sender, EventArgs e)
    {
        page++;
        await Ui.RunAsync(this, LoadAsync, statusLabel);
    }
    private void grid_SelectionChanged(object? sender, EventArgs e) => ShowSelectedUser();
    private void grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) ShowSelectedUser();
    }
    private void ShowSelectedUser()
    {
        if (!ready || grid.CurrentRow?.DataBoundItem is not UserDto user) return;
        fillingForm = true;
        selectedId = user.Id;
        userBox.Text = user.UserName;
        userBox.ReadOnly = true;
        nameBox.Text = user.FullName;
        activeCheck.Checked = user.Active;
        roleCombo.SelectedItem = user.Role;
        passwordBox.Clear();
        passwordBox.Enabled = false;
        fillingForm = false;
    }
    private void newButton_Click(object? sender, EventArgs e)
    {
        fillingForm = true;
        selectedId = null;
        userBox.ReadOnly = false;
        passwordBox.Enabled = true;
        userBox.Clear();
        nameBox.Clear();
        passwordBox.Clear();
        activeCheck.Checked = true;
        if (roleCombo.Items.Count > 0) roleCombo.SelectedIndex = 0;
        fillingForm = false;
        statusLabel.Text = "Nhập tài khoản, họ tên, mật khẩu và vai trò rồi bấm Lưu.";
        userBox.Focus();
    }
    private async void saveButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            var client = Ui.RequireApi(api);
            if (selectedId != null)
                await client.SaveAsync($"api/users/{selectedId}", new UserUpdate(nameBox.Text, roleCombo.Text, activeCheck.Checked), true);
            else
                await client.SaveAsync("api/users", new UserRequest(userBox.Text, nameBox.Text, passwordBox.Text, roleCombo.Text, activeCheck.Checked));
            await LoadAsync();
        }, statusLabel);
    }
    private void ShowEditHint(string hint)
    {
        if (ready && !fillingForm) Ui.InputChanged(statusLabel, hint);
    }
    private void searchBox_TextChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        page = 1;
        prevButton.Enabled = false;
        nextButton.Enabled = false;
        statusLabel.Text = "Nhấn Tìm hoặc Enter để áp dụng từ khóa mới.";
    }
    private void searchBox_KeyDown(object? sender, KeyEventArgs e) => Ui.SearchOnEnter(e, searchButton);
    private void filterRoleCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!ready || fillingForm) return;
        page = 1;
        grid.DataSource = null;
        selectedId = null;
        newButton_Click(sender, e);
        prevButton.Enabled = false;
        nextButton.Enabled = false;
        statusLabel.Text = "Đã đổi bộ lọc vai trò. Nhấn Tìm để tải danh sách mới.";
    }
    private void userBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(string.IsNullOrWhiteSpace(userBox.Text) ? "Tài khoản không được để trống." : "Tên tài khoản phải duy nhất; không đổi sau khi tạo.");
    private void nameBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(string.IsNullOrWhiteSpace(nameBox.Text) ? "Họ tên không được để trống." : "Đã thay đổi họ tên. Bấm Lưu để cập nhật.");
    private void passwordBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(passwordBox.TextLength < 8 ? "Mật khẩu mới cần ít nhất 8 ký tự." : "Mật khẩu còn được kiểm tra theo chính sách Identity khi lưu.");
    private void roleCombo_SelectedIndexChanged(object? sender, EventArgs e) => ShowEditHint("Đã chọn vai trò. Server sẽ kiểm tra quyền cấp vai trò khi lưu.");
    private void activeCheck_CheckedChanged(object? sender, EventArgs e) => ShowEditHint(activeCheck.Checked ? "Tài khoản sẽ hoạt động sau khi lưu." : "Tài khoản sẽ bị khóa sau khi lưu; lịch sử vẫn được giữ.");
}
