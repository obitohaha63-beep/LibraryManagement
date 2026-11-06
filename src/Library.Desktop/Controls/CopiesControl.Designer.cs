#nullable enable
namespace Library.Desktop.Controls;
partial class CopiesControl
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
    private DataGridView grid = null!;
    private Button prevButton = null!;
    private Button nextButton = null!;
    private Label pageLabel = null!;
    private Label bookLabel = null!;
    private TextBox bookIdBox = null!;
    private Label codeLabel = null!;
    private TextBox codeBox = null!;
    private Label locationLabel = null!;
    private TextBox locationBox = null!;
    private Label stateLabel = null!;
    private ComboBox stateCombo = null!;
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
        heading.Size = new Size(900, 27);
        heading.Text = "BẢN SAO SÁCH · MỖI CUỐN MỘT MÃ RIÊNG";
        heading.AutoEllipsis = true;
        
        searchBox = new TextBox();
        searchBox.Name = "searchBox";
        searchBox.Location = new Point(25, 75);
        searchBox.Size = new Size(360, 31);
        
        searchButton = new Button();
        searchButton.Name = "searchButton";
        searchButton.Location = new Point(405, 72);
        searchButton.Size = new Size(145, 38);
        searchButton.Text = "Tìm / tải lại";
        searchButton.BackColor = Color.FromArgb(48, 78, 56);
        searchButton.ForeColor = Color.White;
        searchButton.FlatStyle = FlatStyle.Flat;
        searchButton.FlatAppearance.BorderSize = 0;
        searchButton.Click += searchButton_Click;
        grid = new DataGridView();
        grid.Name = "grid";
        grid.Location = new Point(25, 130);
        grid.Size = new Size(1020, 230);
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grid.SelectionChanged += grid_SelectionChanged;
        prevButton = new Button();
        prevButton.Name = "prevButton";
        prevButton.Location = new Point(25, 375);
        prevButton.Size = new Size(110, 38);
        prevButton.Text = "← Trước";
        prevButton.BackColor = Color.FromArgb(48, 78, 56);
        prevButton.ForeColor = Color.White;
        prevButton.FlatStyle = FlatStyle.Flat;
        prevButton.FlatAppearance.BorderSize = 0;
        prevButton.Click += prevButton_Click;
        nextButton = new Button();
        nextButton.Name = "nextButton";
        nextButton.Location = new Point(145, 375);
        nextButton.Size = new Size(110, 38);
        nextButton.Text = "Sau →";
        nextButton.BackColor = Color.FromArgb(48, 78, 56);
        nextButton.ForeColor = Color.White;
        nextButton.FlatStyle = FlatStyle.Flat;
        nextButton.FlatAppearance.BorderSize = 0;
        nextButton.Click += nextButton_Click;
        pageLabel = new Label();
        pageLabel.Name = "pageLabel";
        pageLabel.Location = new Point(280, 380);
        pageLabel.Size = new Size(420, 27);
        pageLabel.Text = "Trang 1";
        pageLabel.AutoEllipsis = true;
        
        bookLabel = new Label();
        bookLabel.Name = "bookLabel";
        bookLabel.Location = new Point(25, 430);
        bookLabel.Size = new Size(400, 27);
        bookLabel.Text = "ID đầu sách (xem ở tab Đầu sách)";
        bookLabel.AutoEllipsis = true;
        
        bookIdBox = new TextBox();
        bookIdBox.Name = "bookIdBox";
        bookIdBox.Location = new Point(25, 465);
        bookIdBox.Size = new Size(300, 31);
        
        codeLabel = new Label();
        codeLabel.Name = "codeLabel";
        codeLabel.Location = new Point(355, 430);
        codeLabel.Size = new Size(400, 27);
        codeLabel.Text = "Mã bản sao (không đổi sau khi tạo)";
        codeLabel.AutoEllipsis = true;
        
        codeBox = new TextBox();
        codeBox.Name = "codeBox";
        codeBox.Location = new Point(355, 465);
        codeBox.Size = new Size(300, 31);
        
        locationLabel = new Label();
        locationLabel.Name = "locationLabel";
        locationLabel.Location = new Point(25, 520);
        locationLabel.Size = new Size(300, 27);
        locationLabel.Text = "Vị trí / kệ";
        locationLabel.AutoEllipsis = true;
        
        locationBox = new TextBox();
        locationBox.Name = "locationBox";
        locationBox.Location = new Point(25, 555);
        locationBox.Size = new Size(300, 31);
        
        stateLabel = new Label();
        stateLabel.Name = "stateLabel";
        stateLabel.Location = new Point(355, 520);
        stateLabel.Size = new Size(300, 27);
        stateLabel.Text = "Trạng thái";
        stateLabel.AutoEllipsis = true;
        
        stateCombo = new ComboBox();
        stateCombo.Name = "stateCombo";
        stateCombo.Location = new Point(355, 555);
        stateCombo.Size = new Size(300, 32);
        stateCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        stateCombo.Items.AddRange(new object[] { "AVAILABLE", "ON_LOAN", "INACTIVE" });
        stateCombo.SelectedIndex = 0;
        newButton = new Button();
        newButton.Name = "newButton";
        newButton.Location = new Point(700, 460);
        newButton.Size = new Size(220, 38);
        newButton.Text = "Tạo bản sao mới";
        newButton.BackColor = Color.FromArgb(48, 78, 56);
        newButton.ForeColor = Color.White;
        newButton.FlatStyle = FlatStyle.Flat;
        newButton.FlatAppearance.BorderSize = 0;
        newButton.Click += newButton_Click;
        saveButton = new Button();
        saveButton.Name = "saveButton";
        saveButton.Location = new Point(700, 550);
        saveButton.Size = new Size(220, 38);
        saveButton.Text = "Lưu bản sao";
        saveButton.BackColor = Color.FromArgb(48, 78, 56);
        saveButton.ForeColor = Color.White;
        saveButton.FlatStyle = FlatStyle.Flat;
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += saveButton_Click;
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
        Size = new Size(1080, 740);
        Name = "CopiesControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(searchBox);
        Controls.Add(searchButton);
        Controls.Add(grid);
        Controls.Add(prevButton);
        Controls.Add(nextButton);
        Controls.Add(pageLabel);
        Controls.Add(bookLabel);
        Controls.Add(bookIdBox);
        Controls.Add(codeLabel);
        Controls.Add(codeBox);
        Controls.Add(locationLabel);
        Controls.Add(locationBox);
        Controls.Add(stateLabel);
        Controls.Add(stateCombo);
        Controls.Add(newButton);
        Controls.Add(saveButton);
        Controls.Add(statusLabel);

        Load += CopiesControl_Load;
        searchBox.TextChanged += searchBox_TextChanged;
        searchBox.KeyDown += searchBox_KeyDown;
        grid.CellContentClick += grid_CellContentClick;
        bookIdBox.TextChanged += bookIdBox_TextChanged;
        codeBox.TextChanged += codeBox_TextChanged;
        locationBox.TextChanged += locationBox_TextChanged;
        stateCombo.SelectedIndexChanged += stateCombo_SelectedIndexChanged;
        ResumeLayout(false);
        PerformLayout();
    }
}