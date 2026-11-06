using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class MetadataControl : UserControl
{
    private LibraryApiClient? api; private int? selectedId;
    public MetadataControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private string Kind => kindCombo.SelectedIndex switch { 1 => "categories", 2 => "publishers", _ => "authors" };
    private async Task LoadAsync() { var meta = await api!.GetAsync<MetadataDto>("api/books/metadata"); grid.DataSource = Kind switch { "categories" => meta.Categories, "publishers" => meta.Publishers, _ => meta.Authors }; }
    private async void Load_Click(object? s, EventArgs e) => await Ui.RunAsync(loadButton, LoadAsync, statusLabel);
    private void New_Click(object? s, EventArgs e) { selectedId = null; nameBox.Clear(); }
    private void Select_Row(object? s, EventArgs e) { if (grid.CurrentRow?.DataBoundItem is NamedDto n) { selectedId = n.Id; nameBox.Text = n.Name; } }
    private void Kind_Changed(object? s, EventArgs e) { selectedId = null; nameBox.Clear(); grid.DataSource = null; }
    private async void Save_Click(object? s, EventArgs e) => await Ui.RunAsync(saveButton, async () => { await api!.SaveAsync($"api/metadata/{Kind}" + (selectedId.HasValue ? $"/{selectedId}" : ""), new NamedRequest(nameBox.Text), selectedId.HasValue); await LoadAsync(); }, statusLabel);
}
