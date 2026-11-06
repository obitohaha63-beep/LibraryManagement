using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class CatalogControl : UserControl
{
    private LibraryApiClient? api;
    private int page = 1;
    private int? selectedId;
    private bool ready;
    private bool fillingForm;

    public CatalogControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;

    private void CatalogControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        prevButton.Enabled = false;
        nextButton.Enabled = false;
        statusLabel.Text = "Tải danh mục để chọn thể loại, nhà xuất bản và tác giả.";
    }

    private async Task LoadAsync()
    {
        var client = Ui.RequireApi(api);
        fillingForm = true;
        try
        {
            var meta = await client.GetAsync<MetadataDto>("api/books/metadata");
            Ui.Bind(categoryCombo, meta.Categories);
            Ui.Bind(publisherCombo, meta.Publishers);
            authorsList.DataSource = meta.Authors;
            authorsList.DisplayMember = "Name";
            var rows = await client.GetAsync<Page<BookDto>>($"api/books?q={Uri.EscapeDataString(searchBox.Text)}&page={page}");
            grid.DataSource = rows.Items;
            pageLabel.Text = $"Trang {page} · {rows.Total} đầu sách";
            nextButton.Enabled = page * rows.Size < rows.Total;
            prevButton.Enabled = page > 1;
        }
        finally { fillingForm = false; }
        ShowSelectedBook();
    }

    private async void loadButton_Click(object? sender, EventArgs e) => await Ui.RunAsync(this, LoadAsync, statusLabel);
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

    private void grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (!fillingForm) ShowSelectedBook();
    }
    private void grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) ShowSelectedBook();
    }
    private void ShowSelectedBook()
    {
        if (!ready || grid.CurrentRow?.DataBoundItem is not BookDto book) return;
        fillingForm = true;
        try
        {
            selectedId = book.Id;
            titleBox.Text = book.Title;
            isbnBox.Text = book.Isbn;
            yearBox.Text = book.Year.ToString();
            imageBox.Text = book.ImagePath;
            descriptionBox.Text = book.Description;
            activeCheck.Checked = book.Active;
            categoryCombo.SelectedValue = book.CategoryId;
            publisherCombo.SelectedValue = book.PublisherId;
            for (var i = 0; i < authorsList.Items.Count; i++)
                authorsList.SetItemChecked(i, book.AuthorIds.Contains(((NamedDto)authorsList.Items[i]).Id));
        }
        finally { fillingForm = false; }
    }

    private void newButton_Click(object? sender, EventArgs e)
    {
        fillingForm = true;
        selectedId = null;
        titleBox.Clear();
        isbnBox.Clear();
        descriptionBox.Clear();
        yearBox.Text = DateTime.Today.Year.ToString();
        imageBox.Text = "images/books/default.svg";
        activeCheck.Checked = true;
        for (var i = 0; i < authorsList.Items.Count; i++) authorsList.SetItemChecked(i, false);
        fillingForm = false;
        statusLabel.Text = "Đang tạo đầu sách mới. Nhập thông tin rồi bấm Lưu sách.";
        titleBox.Focus();
    }

    private async void saveButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            var request = new BookRequest
            {
                Title = titleBox.Text, Isbn = isbnBox.Text, Year = Ui.Int(yearBox),
                Description = descriptionBox.Text, ImagePath = imageBox.Text,
                CategoryId = categoryCombo.SelectedValue is int category ? category : 0,
                PublisherId = publisherCombo.SelectedValue is int publisher ? publisher : 0,
                AuthorIds = authorsList.CheckedItems.Cast<NamedDto>().Select(x => x.Id).ToArray(),
                Active = activeCheck.Checked
            };
            await Ui.RequireApi(api).SaveAsync(selectedId.HasValue ? $"api/books/{selectedId}" : "api/books", request, selectedId.HasValue);
            await LoadAsync();
        }, statusLabel);
    }

    private async void chooseImageButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh bìa sách",
            Filter = "Ảnh sách (*.png;*.jpg;*.jpeg;*.gif;*.webp)|*.png;*.jpg;*.jpeg;*.gif;*.webp",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await Ui.RunAsync(this, async () =>
        {
            imageBox.Text = await Ui.RequireApi(api).UploadBookImageAsync(dialog.FileName);
        }, statusLabel);
    }

    private async void deleteButton_Click(object? sender, EventArgs e)
    {
        if (selectedId is not int id) { statusLabel.Text = "Hãy chọn đầu sách cần xóa."; return; }
        if (MessageBox.Show(this, "Xóa cứng đầu sách khỏi cơ sở dữ liệu? Thao tác không thể hoàn tác.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        await Ui.RunAsync(this, async () =>
        {
            await Ui.RequireApi(api).DeleteAsync($"api/books/{id}");
            selectedId = null;
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
        statusLabel.Text = "Nhấn Tìm sách hoặc Enter để áp dụng từ khóa mới.";
    }
    private void searchBox_KeyDown(object? sender, KeyEventArgs e) => Ui.SearchOnEnter(e, searchButton);
    private void titleBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(string.IsNullOrWhiteSpace(titleBox.Text) ? "Tên sách không được để trống." : "Đã thay đổi tên sách. Bấm Lưu để cập nhật.");
    private void isbnBox_TextChanged(object? sender, EventArgs e) => ShowEditHint("ISBN là bắt buộc và không được trùng đầu sách khác.");
    private void yearBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(Ui.Int(yearBox) >= 1400 && Ui.Int(yearBox) <= DateTime.Today.Year + 1 ? "Năm xuất bản hợp lệ. Bấm Lưu để cập nhật." : "Nhập năm xuất bản từ 1400 đến năm sau.");
    private void imageBox_TextChanged(object? sender, EventArgs e) => ShowEditHint(LoanRules.IsRelativeBookImage(imageBox.Text) ? "Đường dẫn ảnh tương đối hợp lệ." : "Ảnh phải có đường dẫn tương đối dạng images/books/ten-anh.svg.");
    private void descriptionBox_TextChanged(object? sender, EventArgs e) => ShowEditHint($"Mô tả: {descriptionBox.TextLength}/4000 ký tự. Bấm Lưu để cập nhật.");
    private void categoryCombo_SelectedIndexChanged(object? sender, EventArgs e) => ShowEditHint("Đã chọn thể loại. Bấm Lưu để cập nhật.");
    private void publisherCombo_SelectedIndexChanged(object? sender, EventArgs e) => ShowEditHint("Đã chọn nhà xuất bản. Bấm Lưu để cập nhật.");
    private void activeCheck_CheckedChanged(object? sender, EventArgs e) => ShowEditHint(activeCheck.Checked ? "Sách sẽ hiển thị trên Web sau khi lưu." : "Sách sẽ ngừng hiển thị sau khi lưu; lịch sử được giữ nguyên.");
    private void authorsList_SelectedIndexChanged(object? sender, EventArgs e) => ShowEditHint("Đánh dấu các tác giả của đầu sách; có thể chọn nhiều tác giả.");
    private void authorsList_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        var count = authorsList.CheckedItems.Count;
        if (e.CurrentValue != CheckState.Checked && e.NewValue == CheckState.Checked) count++;
        if (e.CurrentValue == CheckState.Checked && e.NewValue != CheckState.Checked) count--;
        ShowEditHint($"Đã chọn {count} tác giả; đầu sách cần ít nhất một tác giả.");
    }
}
