using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class CatalogControl : UserControl
{
    private LibraryApiClient? api; private int page = 1; private int? selectedId;
    public CatalogControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private async Task LoadAsync()
    {
        var meta = await api!.GetAsync<MetadataDto>("api/books/metadata"); Ui.Bind(categoryCombo, meta.Categories); Ui.Bind(publisherCombo, meta.Publishers);
        authorsList.DataSource = meta.Authors; authorsList.DisplayMember = "Name";
        var rows = await api.GetAsync<Page<BookDto>>($"api/books?q={Uri.EscapeDataString(searchBox.Text)}&page={page}"); grid.DataSource = rows.Items;
        pageLabel.Text = $"Trang {page} · {rows.Total} đầu sách"; nextButton.Enabled = page * rows.Size < rows.Total; prevButton.Enabled = page > 1;
    }
    private async void Load_Click(object? s, EventArgs e) => await Ui.RunAsync(loadButton, LoadAsync, statusLabel);
    private async void Search_Click(object? s, EventArgs e) { page = 1; await Ui.RunAsync(searchButton, LoadAsync, statusLabel); }
    private async void Prev_Click(object? s, EventArgs e) { page = Math.Max(1, page - 1); await Ui.RunAsync(prevButton, LoadAsync, statusLabel); }
    private async void Next_Click(object? s, EventArgs e) { page++; await Ui.RunAsync(nextButton, LoadAsync, statusLabel); }
    private void Select_Row(object? s, EventArgs e)
    {
        if (grid.CurrentRow?.DataBoundItem is not BookDto b) return; selectedId = b.Id; titleBox.Text = b.Title; isbnBox.Text = b.Isbn; yearBox.Text = b.Year.ToString(); imageBox.Text = b.ImagePath; descriptionBox.Text = b.Description; activeCheck.Checked = b.Active;
        categoryCombo.SelectedValue = b.CategoryId; publisherCombo.SelectedValue = b.PublisherId;
        for (var i = 0; i < authorsList.Items.Count; i++) authorsList.SetItemChecked(i, b.AuthorIds.Contains(((NamedDto)authorsList.Items[i]).Id));
    }
    private void New_Click(object? s, EventArgs e) { selectedId = null; titleBox.Clear(); isbnBox.Clear(); descriptionBox.Clear(); yearBox.Text = "2026"; imageBox.Text = "images/books/default.svg"; activeCheck.Checked = true; for (var i = 0; i < authorsList.Items.Count; i++) authorsList.SetItemChecked(i, false); }
    private async void Save_Click(object? s, EventArgs e) => await Ui.RunAsync(saveButton, async () => {
        var r = new BookRequest { Title = titleBox.Text, Isbn = isbnBox.Text, Year = Ui.Int(yearBox), Description = descriptionBox.Text, ImagePath = imageBox.Text,
            CategoryId = categoryCombo.SelectedValue is int c ? c : 0, PublisherId = publisherCombo.SelectedValue is int p ? p : 0, AuthorIds = authorsList.CheckedItems.Cast<NamedDto>().Select(x => x.Id).ToArray(), Active = activeCheck.Checked };
        await api!.SaveAsync(selectedId.HasValue ? $"api/books/{selectedId}" : "api/books", r, selectedId.HasValue); await LoadAsync(); }, statusLabel);
}
