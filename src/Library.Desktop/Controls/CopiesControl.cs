using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class CopiesControl : UserControl
{
    private LibraryApiClient? api;
    private int page = 1;
    private int? selectedId;
    private bool ready;
    private bool fillingForm;

    public CopiesControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private void CopiesControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        prevButton.Enabled = false;
        nextButton.Enabled = false;
        stateCombo.Enabled = false; // New copies always start AVAILABLE.
        statusLabel.Text = "Tìm bản sao để sửa, hoặc bấm Tạo bản sao mới.";
    }
    private async Task LoadAsync()
    {
        var rows = await Ui.RequireApi(api).GetAsync<Page<CopyDto>>($"api/copies?q={Uri.EscapeDataString(searchBox.Text)}&page={page}");
        grid.DataSource = rows.Items;
        pageLabel.Text = $"Trang {page} · {rows.Total} bản sao";
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
    private void grid_SelectionChanged(object? sender, EventArgs e) => ShowSelectedCopy();
    private void grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) ShowSelectedCopy();
    }
    private void ShowSelectedCopy()
    {
        if (!ready || grid.CurrentRow?.DataBoundItem is not CopyDto copy) return;
        fillingForm = true;
        selectedId = copy.Id;
        bookIdBox.Text = copy.BookId.ToString();
        codeBox.Text = copy.Code;
        locationBox.Text = copy.Location;
        stateCombo.SelectedItem = copy.Status;
        bookIdBox.ReadOnly = true;
        codeBox.ReadOnly = true;
        stateCombo.Enabled = copy.Status != "ON_LOAN";
        fillingForm = false;
    }
    private void newButton_Click(object? sender, EventArgs e)
    {
        fillingForm = true;
        selectedId = null;
        bookIdBox.ReadOnly = false;
        codeBox.ReadOnly = false;
        codeBox.Clear();
        locationBox.Clear();
        stateCombo.SelectedItem = "AVAILABLE";
        stateCombo.Enabled = false;
        fillingForm = false;
        statusLabel.Text = "Bản sao mới có trạng thái AVAILABLE; mã bản sao không được tái sử dụng.";
        codeBox.Focus();
    }
    private async void saveButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            var client = Ui.RequireApi(api);
            if (selectedId.HasValue)
                await client.SaveAsync($"api/copies/{selectedId}", new CopyUpdate(locationBox.Text, stateCombo.Text), true);
            else
                await client.SaveAsync("api/copies", new CopyRequest(Ui.Int(bookIdBox), codeBox.Text, locationBox.Text));
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
    private void bookIdBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(Ui.Int(bookIdBox) > 0 ? "ID đầu sách sẽ được kiểm tra tồn tại trên server khi lưu." : "ID đầu sách phải là số nguyên dương; xem ID tại tab Đầu sách.");
    private void codeBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(string.IsNullOrWhiteSpace(codeBox.Text) ? "Mã bản sao không được để trống." : "Mã bản sao phải duy nhất và không đổi sau khi tạo.");
    private void locationBox_TextChanged(object? sender, EventArgs e) => ShowEditHint("Đã thay đổi vị trí/kệ. Bấm Lưu để cập nhật.");
    private void stateCombo_SelectedIndexChanged(object? sender, EventArgs e) => ShowEditHint(stateCombo.Text == "ON_LOAN" ? "ON_LOAN chỉ được thay đổi bằng giao dịch mượn–trả." : "Bấm Lưu để áp dụng trạng thái; server kiểm tra lại điều kiện.");
}
