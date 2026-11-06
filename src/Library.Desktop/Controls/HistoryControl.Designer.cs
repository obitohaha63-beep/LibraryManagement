#nullable enable
namespace Library.Desktop.Controls;
partial class HistoryControl
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
    private CheckBox overdueCheck = null!;
    private CheckBox activeCheck = null!;
    private CheckBox dateCheck = null!;
    private DateTimePicker fromDate = null!;
    private DateTimePicker toDate = null!;
    private Button searchButton = null!;
    private DataGridView grid = null!;
    private Button prevButton = null!;
    private Button nextButton = null!;
    private Label pageLabel = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        heading = new Label();
        heading.Name = "heading";
        heading.Location = new Point(25, 20);
        heading.Size = new Size(900, 27);
        heading.Text = "LỊCH SỬ GIAO DỊCH VÀ SÁCH QUÁ HẠN";
        heading.AutoEllipsis = true;
        
        overdueCheck = new CheckBox();
        overdueCheck.Name = "overdueCheck";
        overdueCheck.Location = new Point(25, 65);
        overdueCheck.Size = new Size(160, 28);
        overdueCheck.Text = "Chỉ quá hạn";
        
        activeCheck = new CheckBox();
        activeCheck.Name = "activeCheck";
        activeCheck.Location = new Point(205, 65);
        activeCheck.Size = new Size(200, 28);
        activeCheck.Text = "Chỉ đang mượn";
        
        dateCheck = new CheckBox();
        dateCheck.Name = "dateCheck";
        dateCheck.Location = new Point(25, 100);
        dateCheck.Size = new Size(180, 28);
        dateCheck.Text = "Lọc ngày mượn";
        
        fromDate = new DateTimePicker();
        fromDate.Name = "fromDate";
        fromDate.Location = new Point(220, 100);
        fromDate.Size = new Size(210, 28);
        fromDate.Format = DateTimePickerFormat.Short;
        toDate = new DateTimePicker();
        toDate.Name = "toDate";
        toDate.Location = new Point(450, 100);
        toDate.Size = new Size(210, 28);
        toDate.Format = DateTimePickerFormat.Short;
        searchButton = new Button();
        searchButton.Name = "searchButton";
        searchButton.Location = new Point(700, 85);
        searchButton.Size = new Size(200, 38);
        searchButton.Text = "Tìm / tải lại";
        searchButton.BackColor = Color.FromArgb(48, 78, 56);
        searchButton.ForeColor = Color.White;
        searchButton.FlatStyle = FlatStyle.Flat;
        searchButton.FlatAppearance.BorderSize = 0;
        searchButton.Click += searchButton_Click;
        grid = new DataGridView();
        grid.Name = "grid";
        grid.Location = new Point(25, 160);
        grid.Size = new Size(1020, 390);
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
        
        prevButton = new Button();
        prevButton.Name = "prevButton";
        prevButton.Location = new Point(25, 580);
        prevButton.Size = new Size(110, 38);
        prevButton.Text = "← Trước";
        prevButton.BackColor = Color.FromArgb(48, 78, 56);
        prevButton.ForeColor = Color.White;
        prevButton.FlatStyle = FlatStyle.Flat;
        prevButton.FlatAppearance.BorderSize = 0;
        prevButton.Click += prevButton_Click;
        nextButton = new Button();
        nextButton.Name = "nextButton";
        nextButton.Location = new Point(145, 580);
        nextButton.Size = new Size(110, 38);
        nextButton.Text = "Sau →";
        nextButton.BackColor = Color.FromArgb(48, 78, 56);
        nextButton.ForeColor = Color.White;
        nextButton.FlatStyle = FlatStyle.Flat;
        nextButton.FlatAppearance.BorderSize = 0;
        nextButton.Click += nextButton_Click;
        pageLabel = new Label();
        pageLabel.Name = "pageLabel";
        pageLabel.Location = new Point(280, 585);
        pageLabel.Size = new Size(600, 27);
        pageLabel.Text = "Trang 1";
        pageLabel.AutoEllipsis = true;
        
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
        Name = "HistoryControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(overdueCheck);
        Controls.Add(activeCheck);
        Controls.Add(dateCheck);
        Controls.Add(fromDate);
        Controls.Add(toDate);
        Controls.Add(searchButton);
        Controls.Add(grid);
        Controls.Add(prevButton);
        Controls.Add(nextButton);
        Controls.Add(pageLabel);
        Controls.Add(statusLabel);

        Load += HistoryControl_Load;
        overdueCheck.CheckedChanged += overdueCheck_CheckedChanged;
        activeCheck.CheckedChanged += activeCheck_CheckedChanged;
        dateCheck.CheckedChanged += dateCheck_CheckedChanged;
        fromDate.ValueChanged += fromDate_ValueChanged;
        toDate.ValueChanged += toDate_ValueChanged;
        grid.CellContentClick += grid_CellContentClick;
        ResumeLayout(false);
        PerformLayout();
    }
}