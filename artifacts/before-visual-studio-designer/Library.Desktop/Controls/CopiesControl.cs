using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class CopiesControl : UserControl
{
    private LibraryApiClient? api; private int page = 1; private int? selectedId;
    public CopiesControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private async Task LoadAsync() { var rows = await api!.GetAsync<Page<CopyDto>>($"api/copies?q={Uri.EscapeDataString(searchBox.Text)}&page={page}"); grid.DataSource = rows.Items; pageLabel.Text = $"Trang {page} · {rows.Total} bản sao"; prevButton.Enabled = page > 1; nextButton.Enabled = page * rows.Size < rows.Total; }
    private async void Search_Click(object? s, EventArgs e) { page = 1; await Ui.RunAsync(searchButton, LoadAsync, statusLabel); }
    private async void Prev_Click(object? s, EventArgs e) { page = Math.Max(1, page - 1); await Ui.RunAsync(prevButton, LoadAsync, statusLabel); }
    private async void Next_Click(object? s, EventArgs e) { page++; await Ui.RunAsync(nextButton, LoadAsync, statusLabel); }
    private void Select_Row(object? s, EventArgs e) { if (grid.CurrentRow?.DataBoundItem is not CopyDto c) return; selectedId = c.Id; bookIdBox.Text = c.BookId.ToString(); codeBox.Text = c.Code; locationBox.Text = c.Location; stateCombo.SelectedItem = c.Status; }
    private void New_Click(object? s, EventArgs e) { selectedId = null; codeBox.Clear(); stateCombo.SelectedItem = "AVAILABLE"; }
    private async void Save_Click(object? s, EventArgs e) => await Ui.RunAsync(saveButton, async () => {
        if (selectedId.HasValue) await api!.SaveAsync($"api/copies/{selectedId}", new CopyUpdate(locationBox.Text, stateCombo.Text), true);
        else await api!.SaveAsync("api/copies", new CopyRequest(Ui.Int(bookIdBox), codeBox.Text, locationBox.Text)); await LoadAsync(); }, statusLabel);
}
