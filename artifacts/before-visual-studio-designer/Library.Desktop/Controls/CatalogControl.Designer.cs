#nullable enable
namespace Library.Desktop.Controls;
partial class CatalogControl
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private Label heading = null!;
    private TextBox searchBox = null!;
    private Button searchButton = null!;
    private Button loadButton = null!;
    private DataGridView grid = null!;
    private Button prevButton = null!;
    private Button nextButton = null!;
    private Label pageLabel = null!;
    private Label titleLabel = null!;
    private TextBox titleBox = null!;
    private Label isbnLabel = null!;
    private TextBox isbnBox = null!;
    private Label yearLabel = null!;
    private TextBox yearBox = null!;
    private Label categoryLabel = null!;
    private ComboBox categoryCombo = null!;
    private Label publisherLabel = null!;
    private ComboBox publisherCombo = null!;
    private Label authorLabel = null!;
    private CheckedListBox authorsList = null!;
    private Label imageLabel = null!;
    private TextBox imageBox = null!;
    private Label descriptionLabel = null!;
    private TextBox descriptionBox = null!;
    private CheckBox activeCheck = null!;
    private Button newButton = null!;
    private Button saveButton = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        heading = new Label();
        heading.Name = "heading";
        heading.Location = new Point(25, 20);
        heading.Size = new Size(800, 27);
        heading.Text = "ĐẦU SÁCH · TÌM KIẾM VÀ BIÊN TẬP";
        heading.AutoEllipsis = true;
        
        searchBox = new TextBox();
        searchBox.Name = "searchBox";
        searchBox.Location = new Point(25, 75);
        searchBox.Size = new Size(360, 31);
        
        searchButton = new Button();
        searchButton.Name = "searchButton";
        searchButton.Location = new Point(405, 72);
        searchButton.Size = new Size(145, 38);
        searchButton.Text = "Tìm sách";
        searchButton.BackColor = Color.FromArgb(48, 78, 56); searchButton.ForeColor = Color.White; searchButton.FlatStyle = FlatStyle.Flat; searchButton.FlatAppearance.BorderSize = 0;
        searchButton.Click += Search_Click;
        loadButton = new Button();
        loadButton.Name = "loadButton";
        loadButton.Location = new Point(565, 72);
        loadButton.Size = new Size(145, 38);
        loadButton.Text = "Tải danh mục";
        loadButton.BackColor = Color.FromArgb(48, 78, 56); loadButton.ForeColor = Color.White; loadButton.FlatStyle = FlatStyle.Flat; loadButton.FlatAppearance.BorderSize = 0;
        loadButton.Click += Load_Click;
        grid = new DataGridView();
        grid.Name = "grid";
        grid.Location = new Point(25, 130);
        grid.Size = new Size(1020, 230);
        grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells; grid.BackgroundColor = Color.White; grid.BorderStyle = BorderStyle.None; grid.RowHeadersVisible = false; grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grid.SelectionChanged += Select_Row;
        prevButton = new Button();
        prevButton.Name = "prevButton";
        prevButton.Location = new Point(25, 375);
        prevButton.Size = new Size(110, 38);
        prevButton.Text = "← Trước";
        prevButton.BackColor = Color.FromArgb(48, 78, 56); prevButton.ForeColor = Color.White; prevButton.FlatStyle = FlatStyle.Flat; prevButton.FlatAppearance.BorderSize = 0;
        prevButton.Click += Prev_Click;
        nextButton = new Button();
        nextButton.Name = "nextButton";
        nextButton.Location = new Point(145, 375);
        nextButton.Size = new Size(110, 38);
        nextButton.Text = "Sau →";
        nextButton.BackColor = Color.FromArgb(48, 78, 56); nextButton.ForeColor = Color.White; nextButton.FlatStyle = FlatStyle.Flat; nextButton.FlatAppearance.BorderSize = 0;
        nextButton.Click += Next_Click;
        pageLabel = new Label();
        pageLabel.Name = "pageLabel";
        pageLabel.Location = new Point(280, 380);
        pageLabel.Size = new Size(420, 27);
        pageLabel.Text = "Trang 1";
        pageLabel.AutoEllipsis = true;
        
        titleLabel = new Label();
        titleLabel.Name = "titleLabel";
        titleLabel.Location = new Point(25, 425);
        titleLabel.Size = new Size(300, 27);
        titleLabel.Text = "Tên sách";
        titleLabel.AutoEllipsis = true;
        
        titleBox = new TextBox();
        titleBox.Name = "titleBox";
        titleBox.Location = new Point(25, 455);
        titleBox.Size = new Size(330, 31);
        
        isbnLabel = new Label();
        isbnLabel.Name = "isbnLabel";
        isbnLabel.Location = new Point(375, 425);
        isbnLabel.Size = new Size(300, 27);
        isbnLabel.Text = "ISBN";
        isbnLabel.AutoEllipsis = true;
        
        isbnBox = new TextBox();
        isbnBox.Name = "isbnBox";
        isbnBox.Location = new Point(375, 455);
        isbnBox.Size = new Size(200, 31);
        
        yearLabel = new Label();
        yearLabel.Name = "yearLabel";
        yearLabel.Location = new Point(595, 425);
        yearLabel.Size = new Size(80, 27);
        yearLabel.Text = "Năm";
        yearLabel.AutoEllipsis = true;
        
        yearBox = new TextBox();
        yearBox.Name = "yearBox";
        yearBox.Location = new Point(595, 455);
        yearBox.Size = new Size(80, 31);
        yearBox.Text = "2026";
        
        categoryLabel = new Label();
        categoryLabel.Name = "categoryLabel";
        categoryLabel.Location = new Point(25, 500);
        categoryLabel.Size = new Size(300, 27);
        categoryLabel.Text = "Thể loại";
        categoryLabel.AutoEllipsis = true;
        
        categoryCombo = new ComboBox();
        categoryCombo.Name = "categoryCombo";
        categoryCombo.Location = new Point(25, 530);
        categoryCombo.Size = new Size(240, 32);
        categoryCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        
        publisherLabel = new Label();
        publisherLabel.Name = "publisherLabel";
        publisherLabel.Location = new Point(285, 500);
        publisherLabel.Size = new Size(300, 27);
        publisherLabel.Text = "Nhà xuất bản";
        publisherLabel.AutoEllipsis = true;
        
        publisherCombo = new ComboBox();
        publisherCombo.Name = "publisherCombo";
        publisherCombo.Location = new Point(285, 530);
        publisherCombo.Size = new Size(240, 32);
        publisherCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        
        authorLabel = new Label();
        authorLabel.Name = "authorLabel";
        authorLabel.Location = new Point(700, 425);
        authorLabel.Size = new Size(300, 27);
        authorLabel.Text = "Tác giả (chọn nhiều)";
        authorLabel.AutoEllipsis = true;
        
        authorsList = new CheckedListBox();
        authorsList.Name = "authorsList";
        authorsList.Location = new Point(700, 455);
        authorsList.Size = new Size(330, 112);
        authorsList.CheckOnClick = true;
        imageLabel = new Label();
        imageLabel.Name = "imageLabel";
        imageLabel.Location = new Point(25, 575);
        imageLabel.Size = new Size(400, 27);
        imageLabel.Text = "Ảnh tương đối (images/books/...)";
        imageLabel.AutoEllipsis = true;
        
        imageBox = new TextBox();
        imageBox.Name = "imageBox";
        imageBox.Location = new Point(25, 605);
        imageBox.Size = new Size(460, 31);
        imageBox.Text = "images/books/default.svg";
        
        descriptionLabel = new Label();
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Location = new Point(25, 730);
        descriptionLabel.Size = new Size(300, 27);
        descriptionLabel.Text = "Mô tả";
        descriptionLabel.AutoEllipsis = true;
        
        descriptionBox = new TextBox();
        descriptionBox.Name = "descriptionBox";
        descriptionBox.Location = new Point(25, 760);
        descriptionBox.Size = new Size(1005, 100);
        descriptionBox.Multiline = true;
        activeCheck = new CheckBox();
        activeCheck.Name = "activeCheck";
        activeCheck.Location = new Point(550, 605);
        activeCheck.Size = new Size(300, 28);
        activeCheck.Text = "Đang hiển thị trên Web";
        activeCheck.Checked = true;
        newButton = new Button();
        newButton.Name = "newButton";
        newButton.Location = new Point(700, 645);
        newButton.Size = new Size(145, 38);
        newButton.Text = "Tạo mới";
        newButton.BackColor = Color.FromArgb(48, 78, 56); newButton.ForeColor = Color.White; newButton.FlatStyle = FlatStyle.Flat; newButton.FlatAppearance.BorderSize = 0;
        newButton.Click += New_Click;
        saveButton = new Button();
        saveButton.Name = "saveButton";
        saveButton.Location = new Point(865, 645);
        saveButton.Size = new Size(145, 38);
        saveButton.Text = "Lưu sách";
        saveButton.BackColor = Color.FromArgb(48, 78, 56); saveButton.ForeColor = Color.White; saveButton.FlatStyle = FlatStyle.Flat; saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += Save_Click;
        statusLabel = new Label();
        statusLabel.Name = "statusLabel";
        statusLabel.Location = new Point(25, 680);
        statusLabel.Size = new Size(1020, 27);
        statusLabel.Text = "Sẵn sàng. Chọn tải dữ liệu để bắt đầu.";
        statusLabel.AutoEllipsis = true;
        

        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(248, 248, 242);
        ForeColor = Color.FromArgb(36, 56, 47);
        Size = new Size(1080, 900);
        Name = "CatalogControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(searchBox);
        Controls.Add(searchButton);
        Controls.Add(loadButton);
        Controls.Add(grid);
        Controls.Add(prevButton);
        Controls.Add(nextButton);
        Controls.Add(pageLabel);
        Controls.Add(titleLabel);
        Controls.Add(titleBox);
        Controls.Add(isbnLabel);
        Controls.Add(isbnBox);
        Controls.Add(yearLabel);
        Controls.Add(yearBox);
        Controls.Add(categoryLabel);
        Controls.Add(categoryCombo);
        Controls.Add(publisherLabel);
        Controls.Add(publisherCombo);
        Controls.Add(authorLabel);
        Controls.Add(authorsList);
        Controls.Add(imageLabel);
        Controls.Add(imageBox);
        Controls.Add(descriptionLabel);
        Controls.Add(descriptionBox);
        Controls.Add(activeCheck);
        Controls.Add(newButton);
        Controls.Add(saveButton);
        Controls.Add(statusLabel);

        ResumeLayout(false);
        PerformLayout();
    }
}