#nullable enable
namespace Library.Desktop.Controls;
partial class CatalogControl
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
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
    private Button chooseImageButton = null!;
    private Label descriptionLabel = null!;
    private TextBox descriptionBox = null!;
    private CheckBox activeCheck = null!;
    private Button newButton = null!;
    private Button saveButton = null!;
    private Button deleteButton = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        heading = new Label();
        searchBox = new TextBox();
        searchButton = new Button();
        loadButton = new Button();
        grid = new DataGridView();
        prevButton = new Button();
        nextButton = new Button();
        pageLabel = new Label();
        titleLabel = new Label();
        titleBox = new TextBox();
        isbnLabel = new Label();
        isbnBox = new TextBox();
        yearLabel = new Label();
        yearBox = new TextBox();
        categoryLabel = new Label();
        categoryCombo = new ComboBox();
        publisherLabel = new Label();
        publisherCombo = new ComboBox();
        authorLabel = new Label();
        authorsList = new CheckedListBox();
        imageLabel = new Label();
        imageBox = new TextBox();
        chooseImageButton = new Button();
        deleteButton = new Button();
        descriptionLabel = new Label();
        descriptionBox = new TextBox();
        activeCheck = new CheckBox();
        newButton = new Button();
        saveButton = new Button();
        statusLabel = new Label();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        SuspendLayout();
        // 
        // heading
        // 
        heading.AutoEllipsis = true;
        heading.Location = new Point(25, 20);
        heading.Name = "heading";
        heading.Size = new Size(800, 27);
        heading.TabIndex = 0;
        heading.Text = "ĐẦU SÁCH · TÌM KIẾM VÀ BIÊN TẬP";
        // 
        // searchBox
        // 
        searchBox.Location = new Point(25, 75);
        searchBox.Name = "searchBox";
        searchBox.Size = new Size(360, 30);
        searchBox.TabIndex = 1;
        searchBox.TextChanged += searchBox_TextChanged;
        searchBox.KeyDown += searchBox_KeyDown;
        // 
        // searchButton
        // 
        searchButton.BackColor = Color.FromArgb(48, 78, 56);
        searchButton.FlatAppearance.BorderSize = 0;
        searchButton.FlatStyle = FlatStyle.Flat;
        searchButton.ForeColor = Color.White;
        searchButton.Location = new Point(405, 72);
        searchButton.Name = "searchButton";
        searchButton.Size = new Size(145, 38);
        searchButton.TabIndex = 2;
        searchButton.Text = "Tìm sách";
        searchButton.UseVisualStyleBackColor = false;
        searchButton.Click += searchButton_Click;
        // 
        // loadButton
        // 
        loadButton.BackColor = Color.FromArgb(48, 78, 56);
        loadButton.FlatAppearance.BorderSize = 0;
        loadButton.FlatStyle = FlatStyle.Flat;
        loadButton.ForeColor = Color.White;
        loadButton.Location = new Point(565, 72);
        loadButton.Name = "loadButton";
        loadButton.Size = new Size(145, 38);
        loadButton.TabIndex = 3;
        loadButton.Text = "Tải danh mục";
        loadButton.UseVisualStyleBackColor = false;
        loadButton.Click += loadButton_Click;
        // 
        // grid
        // 
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.ColumnHeadersHeight = 29;
        grid.Location = new Point(25, 130);
        grid.MultiSelect = false;
        grid.Name = "grid";
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.RowHeadersWidth = 51;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.Size = new Size(1020, 230);
        grid.TabIndex = 4;
        grid.CellContentClick += grid_CellContentClick;
        grid.SelectionChanged += grid_SelectionChanged;
        // 
        // prevButton
        // 
        prevButton.BackColor = Color.FromArgb(48, 78, 56);
        prevButton.FlatAppearance.BorderSize = 0;
        prevButton.FlatStyle = FlatStyle.Flat;
        prevButton.ForeColor = Color.White;
        prevButton.Location = new Point(25, 375);
        prevButton.Name = "prevButton";
        prevButton.Size = new Size(110, 38);
        prevButton.TabIndex = 5;
        prevButton.Text = "← Trước";
        prevButton.UseVisualStyleBackColor = false;
        prevButton.Click += prevButton_Click;
        // 
        // nextButton
        // 
        nextButton.BackColor = Color.FromArgb(48, 78, 56);
        nextButton.FlatAppearance.BorderSize = 0;
        nextButton.FlatStyle = FlatStyle.Flat;
        nextButton.ForeColor = Color.White;
        nextButton.Location = new Point(145, 375);
        nextButton.Name = "nextButton";
        nextButton.Size = new Size(110, 38);
        nextButton.TabIndex = 6;
        nextButton.Text = "Sau →";
        nextButton.UseVisualStyleBackColor = false;
        nextButton.Click += nextButton_Click;
        // 
        // pageLabel
        // 
        pageLabel.AutoEllipsis = true;
        pageLabel.Location = new Point(280, 380);
        pageLabel.Name = "pageLabel";
        pageLabel.Size = new Size(420, 27);
        pageLabel.TabIndex = 7;
        pageLabel.Text = "Trang 1";
        // 
        // titleLabel
        // 
        titleLabel.AutoEllipsis = true;
        titleLabel.Location = new Point(25, 425);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(300, 27);
        titleLabel.TabIndex = 8;
        titleLabel.Text = "Tên sách";
        // 
        // titleBox
        // 
        titleBox.Location = new Point(25, 455);
        titleBox.Name = "titleBox";
        titleBox.Size = new Size(330, 30);
        titleBox.TabIndex = 9;
        titleBox.TextChanged += titleBox_TextChanged;
        // 
        // isbnLabel
        // 
        isbnLabel.AutoEllipsis = true;
        isbnLabel.Location = new Point(375, 425);
        isbnLabel.Name = "isbnLabel";
        isbnLabel.Size = new Size(300, 27);
        isbnLabel.TabIndex = 10;
        isbnLabel.Text = "ISBN";
        // 
        // isbnBox
        // 
        isbnBox.Location = new Point(375, 455);
        isbnBox.Name = "isbnBox";
        isbnBox.Size = new Size(200, 30);
        isbnBox.TabIndex = 11;
        isbnBox.TextChanged += isbnBox_TextChanged;
        // 
        // yearLabel
        // 
        yearLabel.AutoEllipsis = true;
        yearLabel.Location = new Point(595, 425);
        yearLabel.Name = "yearLabel";
        yearLabel.Size = new Size(80, 27);
        yearLabel.TabIndex = 12;
        yearLabel.Text = "Năm";
        // 
        // yearBox
        // 
        yearBox.Location = new Point(595, 455);
        yearBox.Name = "yearBox";
        yearBox.Size = new Size(80, 30);
        yearBox.TabIndex = 13;
        yearBox.Text = "2026";
        yearBox.TextChanged += yearBox_TextChanged;
        // 
        // categoryLabel
        // 
        categoryLabel.AutoEllipsis = true;
        categoryLabel.Location = new Point(25, 500);
        categoryLabel.Name = "categoryLabel";
        categoryLabel.Size = new Size(300, 27);
        categoryLabel.TabIndex = 14;
        categoryLabel.Text = "Thể loại";
        // 
        // categoryCombo
        // 
        categoryCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        categoryCombo.Location = new Point(25, 530);
        categoryCombo.Name = "categoryCombo";
        categoryCombo.Size = new Size(240, 31);
        categoryCombo.TabIndex = 15;
        categoryCombo.SelectedIndexChanged += categoryCombo_SelectedIndexChanged;
        // 
        // publisherLabel
        // 
        publisherLabel.AutoEllipsis = true;
        publisherLabel.Location = new Point(285, 500);
        publisherLabel.Name = "publisherLabel";
        publisherLabel.Size = new Size(300, 27);
        publisherLabel.TabIndex = 16;
        publisherLabel.Text = "Nhà xuất bản";
        // 
        // publisherCombo
        // 
        publisherCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        publisherCombo.Location = new Point(285, 530);
        publisherCombo.Name = "publisherCombo";
        publisherCombo.Size = new Size(240, 31);
        publisherCombo.TabIndex = 17;
        publisherCombo.SelectedIndexChanged += publisherCombo_SelectedIndexChanged;
        // 
        // authorLabel
        // 
        authorLabel.AutoEllipsis = true;
        authorLabel.Location = new Point(700, 425);
        authorLabel.Name = "authorLabel";
        authorLabel.Size = new Size(300, 27);
        authorLabel.TabIndex = 18;
        authorLabel.Text = "Tác giả (chọn nhiều)";
        // 
        // authorsList
        // 
        authorsList.CheckOnClick = true;
        authorsList.Location = new Point(700, 455);
        authorsList.Name = "authorsList";
        authorsList.Size = new Size(330, 104);
        authorsList.TabIndex = 19;
        authorsList.ItemCheck += authorsList_ItemCheck;
        authorsList.SelectedIndexChanged += authorsList_SelectedIndexChanged;
        // 
        // imageLabel
        // 
        imageLabel.AutoEllipsis = true;
        imageLabel.Location = new Point(25, 575);
        imageLabel.Name = "imageLabel";
        imageLabel.Size = new Size(400, 27);
        imageLabel.TabIndex = 20;
        imageLabel.Text = "Ảnh tương đối (images/books/...)";
        // 
        // imageBox
        // 
        imageBox.Location = new Point(25, 605);
        imageBox.Name = "imageBox";
        imageBox.Size = new Size(460, 30);
        imageBox.TabIndex = 21;
        imageBox.Text = "images/books/default.svg";
        imageBox.TextChanged += imageBox_TextChanged;
        // 
        // chooseImageButton
        // 
        chooseImageButton.Location = new Point(500, 602);
        chooseImageButton.Name = "chooseImageButton";
        chooseImageButton.Size = new Size(150, 38);
        chooseImageButton.TabIndex = 22;
        chooseImageButton.Text = "Chọn ảnh...";
        chooseImageButton.Click += chooseImageButton_Click;
        // 
        // deleteButton
        // 
        deleteButton.Location = new Point(700, 690);
        deleteButton.Name = "deleteButton";
        deleteButton.Size = new Size(145, 38);
        deleteButton.TabIndex = 28;
        deleteButton.Text = "Xóa cứng";
        deleteButton.Click += deleteButton_Click;
        // 
        // descriptionLabel
        // 
        descriptionLabel.AutoEllipsis = true;
        descriptionLabel.Location = new Point(25, 730);
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Size = new Size(300, 27);
        descriptionLabel.TabIndex = 23;
        descriptionLabel.Text = "Mô tả";
        // 
        // descriptionBox
        // 
        descriptionBox.Location = new Point(25, 760);
        descriptionBox.Multiline = true;
        descriptionBox.Name = "descriptionBox";
        descriptionBox.Size = new Size(1005, 100);
        descriptionBox.TabIndex = 24;
        descriptionBox.TextChanged += descriptionBox_TextChanged;
        // 
        // activeCheck
        // 
        activeCheck.Checked = true;
        activeCheck.CheckState = CheckState.Checked;
        activeCheck.Location = new Point(25, 651);
        activeCheck.Name = "activeCheck";
        activeCheck.Size = new Size(300, 28);
        activeCheck.TabIndex = 25;
        activeCheck.Text = "Đang hiển thị trên Web";
        activeCheck.CheckedChanged += activeCheck_CheckedChanged;
        // 
        // newButton
        // 
        newButton.BackColor = Color.FromArgb(48, 78, 56);
        newButton.FlatAppearance.BorderSize = 0;
        newButton.FlatStyle = FlatStyle.Flat;
        newButton.ForeColor = Color.White;
        newButton.Location = new Point(700, 645);
        newButton.Name = "newButton";
        newButton.Size = new Size(145, 38);
        newButton.TabIndex = 26;
        newButton.Text = "Tạo mới";
        newButton.UseVisualStyleBackColor = false;
        newButton.Click += newButton_Click;
        // 
        // saveButton
        // 
        saveButton.BackColor = Color.FromArgb(48, 78, 56);
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.FlatStyle = FlatStyle.Flat;
        saveButton.ForeColor = Color.White;
        saveButton.Location = new Point(865, 645);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(145, 38);
        saveButton.TabIndex = 27;
        saveButton.Text = "Lưu sách";
        saveButton.UseVisualStyleBackColor = false;
        saveButton.Click += saveButton_Click;
        // 
        // statusLabel
        // 
        statusLabel.AutoEllipsis = true;
        statusLabel.Location = new Point(25, 680);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1020, 27);
        statusLabel.TabIndex = 29;
        statusLabel.Text = "Sẵn sàng. Chọn tải dữ liệu để bắt đầu.";
        // 
        // CatalogControl
        // 
        AutoScaleDimensions = new SizeF(9F, 23F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        BackColor = Color.FromArgb(248, 248, 242);
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
        Controls.Add(chooseImageButton);
        Controls.Add(descriptionLabel);
        Controls.Add(descriptionBox);
        Controls.Add(activeCheck);
        Controls.Add(newButton);
        Controls.Add(saveButton);
        Controls.Add(deleteButton);
        Controls.Add(statusLabel);
        Font = new Font("Segoe UI", 10F);
        ForeColor = Color.FromArgb(36, 56, 47);
        Name = "CatalogControl";
        Size = new Size(1080, 900);
        Load += CatalogControl_Load;
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
