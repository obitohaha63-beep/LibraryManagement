using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class MetadataControl : UserControl
{
    private LibraryApiClient? api;
    private int? selectedId;
    private bool ready;

    public MetadataControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private string Kind => kindCombo.SelectedIndex switch { 1 => "categories", 2 => "publishers", _ => "authors" };

    private void MetadataControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        saveButton.Enabled = !string.IsNullOrWhiteSpace(nameBox.Text);
        statusLabel.Text = "Chọn loại danh mục rồi bấm Tải danh mục.";
    }
    private async Task LoadAsync()
    {
        var meta = await Ui.RequireApi(api).GetAsync<MetadataDto>("api/books/metadata");
        grid.DataSource = Kind switch { "categories" => meta.Categories, "publishers" => meta.Publishers, _ => meta.Authors };
    }
    private async void loadButton_Click(object? sender, EventArgs e) => await Ui.RunAsync(this, LoadAsync, statusLabel);
    private void newButton_Click(object? sender, EventArgs e)
    {
        selectedId = null;
        nameBox.Clear();
        statusLabel.Text = "Nhập tên danh mục mới rồi bấm Lưu.";
        nameBox.Focus();
    }
    private void grid_SelectionChanged(object? sender, EventArgs e) => ShowSelectedItem();
    private void grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) ShowSelectedItem();
    }
    private void ShowSelectedItem()
    {
        if (!ready || grid.CurrentRow?.DataBoundItem is not NamedDto item) return;
        selectedId = item.Id;
        nameBox.Text = item.Name;
    }
    private void kindCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        selectedId = null;
        nameBox.Clear();
        grid.DataSource = null;
        statusLabel.Text = "Đã đổi loại danh mục. Bấm Tải danh mục để lấy dữ liệu.";
    }
    private void nameBox_TextChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        saveButton.Enabled = !string.IsNullOrWhiteSpace(nameBox.Text) && nameBox.TextLength <= 120;
        statusLabel.Text = saveButton.Enabled ? "Tên hợp lệ. Bấm Lưu để cập nhật danh mục." : "Tên danh mục cần từ 1 đến 120 ký tự.";
    }
    private async void saveButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            await Ui.RequireApi(api).SaveAsync($"api/metadata/{Kind}" + (selectedId.HasValue ? $"/{selectedId}" : ""), new NamedRequest(nameBox.Text), selectedId.HasValue);
            await LoadAsync();
        }, statusLabel);
    }
}
