#nullable enable
namespace Library.Desktop.Controls;
partial class CirculationControl
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private Label heading = null!;
    private Label hint = null!;
    private Label readerLabel = null!;
    private TextBox readerSearch = null!;
    private Button findButton = null!;
    private ComboBox readerCombo = null!;
    private Label codesLabel = null!;
    private TextBox codesBox = null!;
    private Button borrowButton = null!;
    private Label receiptLabel = null!;
    private Label returnTitle = null!;
    private TextBox returnBox = null!;
    private Button returnButton = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        heading = new Label();
        heading.Name = "heading";
        heading.Location = new Point(25, 25);
        heading.Size = new Size(800, 27);
        heading.Text = "MƯỢN VÀ TRẢ SÁCH";
        heading.AutoEllipsis = true;
        
        hint = new Label();
        hint.Name = "hint";
        hint.Location = new Point(25, 65);
        hint.Size = new Size(1000, 27);
        hint.Text = "Tìm bạn đọc theo mã tài khoản. Nhập nhiều mã bản sao cách nhau bằng dấu phẩy.";
        hint.AutoEllipsis = true;
        
        readerLabel = new Label();
        readerLabel.Name = "readerLabel";
        readerLabel.Location = new Point(25, 125);
        readerLabel.Size = new Size(300, 27);
        readerLabel.Text = "Mã hoặc tên bạn đọc";
        readerLabel.AutoEllipsis = true;
        
        readerSearch = new TextBox();
        readerSearch.Name = "readerSearch";
        readerSearch.Location = new Point(25, 155);
        readerSearch.Size = new Size(280, 31);
        
        findButton = new Button();
        findButton.Name = "findButton";
        findButton.Location = new Point(325, 152);
        findButton.Size = new Size(145, 38);
        findButton.Text = "Tìm bạn đọc";
        findButton.BackColor = Color.FromArgb(48, 78, 56); findButton.ForeColor = Color.White; findButton.FlatStyle = FlatStyle.Flat; findButton.FlatAppearance.BorderSize = 0;
        findButton.Click += Find_Click;
        readerCombo = new ComboBox();
        readerCombo.Name = "readerCombo";
        readerCombo.Location = new Point(25, 210);
        readerCombo.Size = new Size(445, 32);
        readerCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        
        codesLabel = new Label();
        codesLabel.Name = "codesLabel";
        codesLabel.Location = new Point(25, 270);
        codesLabel.Size = new Size(650, 27);
        codesLabel.Text = "Mã bản sao cần mượn (ví dụ CP001, CP002)";
        codesLabel.AutoEllipsis = true;
        
        codesBox = new TextBox();
        codesBox.Name = "codesBox";
        codesBox.Location = new Point(25, 305);
        codesBox.Size = new Size(650, 100);
        codesBox.Multiline = true;
        borrowButton = new Button();
        borrowButton.Name = "borrowButton";
        borrowButton.Location = new Point(25, 425);
        borrowButton.Size = new Size(220, 38);
        borrowButton.Text = "Xác nhận mượn";
        borrowButton.BackColor = Color.FromArgb(48, 78, 56); borrowButton.ForeColor = Color.White; borrowButton.FlatStyle = FlatStyle.Flat; borrowButton.FlatAppearance.BorderSize = 0;
        borrowButton.Click += Borrow_Click;
        receiptLabel = new Label();
        receiptLabel.Name = "receiptLabel";
        receiptLabel.Location = new Point(280, 430);
        receiptLabel.Size = new Size(700, 27);
        receiptLabel.AutoEllipsis = true;
        
        returnTitle = new Label();
        returnTitle.Name = "returnTitle";
        returnTitle.Location = new Point(25, 500);
        returnTitle.Size = new Size(650, 27);
        returnTitle.Text = "NHẬN TRẢ TỪNG BẢN SAO";
        returnTitle.AutoEllipsis = true;
        
        returnBox = new TextBox();
        returnBox.Name = "returnBox";
        returnBox.Location = new Point(25, 545);
        returnBox.Size = new Size(300, 31);
        
        returnButton = new Button();
        returnButton.Name = "returnButton";
        returnButton.Location = new Point(345, 542);
        returnButton.Size = new Size(220, 38);
        returnButton.Text = "Xác nhận trả";
        returnButton.BackColor = Color.FromArgb(48, 78, 56); returnButton.ForeColor = Color.White; returnButton.FlatStyle = FlatStyle.Flat; returnButton.FlatAppearance.BorderSize = 0;
        returnButton.Click += Return_Click;
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
        Name = "CirculationControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(hint);
        Controls.Add(readerLabel);
        Controls.Add(readerSearch);
        Controls.Add(findButton);
        Controls.Add(readerCombo);
        Controls.Add(codesLabel);
        Controls.Add(codesBox);
        Controls.Add(borrowButton);
        Controls.Add(receiptLabel);
        Controls.Add(returnTitle);
        Controls.Add(returnBox);
        Controls.Add(returnButton);
        Controls.Add(statusLabel);

        ResumeLayout(false);
        PerformLayout();
    }
}