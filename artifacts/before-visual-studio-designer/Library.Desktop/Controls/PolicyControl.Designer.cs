#nullable enable
namespace Library.Desktop.Controls;
partial class PolicyControl
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private Label heading = null!;
    private Label hint = null!;
    private Label limitLabel = null!;
    private NumericUpDown limitBox = null!;
    private Label daysLabel = null!;
    private NumericUpDown daysBox = null!;
    private CheckBox overdueCheck = null!;
    private Button loadButton = null!;
    private Button saveButton = null!;
    private Label versionLabel = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        heading = new Label();
        heading.Name = "heading";
        heading.Location = new Point(25, 25);
        heading.Size = new Size(900, 27);
        heading.Text = "CHÍNH SÁCH MƯỢN · CHỈ QUẢN LÝ";
        heading.AutoEllipsis = true;
        
        hint = new Label();
        hint.Name = "hint";
        hint.Location = new Point(25, 75);
        hint.Size = new Size(1000, 27);
        hint.Text = "Chính sách thử nghiệm. Mỗi lần lưu tạo phiên bản mới, chỉ áp dụng cho phiếu mượn mới.";
        hint.AutoEllipsis = true;
        
        limitLabel = new Label();
        limitLabel.Name = "limitLabel";
        limitLabel.Location = new Point(25, 150);
        limitLabel.Size = new Size(450, 27);
        limitLabel.Text = "Giới hạn bản sao đang mượn";
        limitLabel.AutoEllipsis = true;
        
        limitBox = new NumericUpDown();
        limitBox.Name = "limitBox";
        limitBox.Location = new Point(25, 185);
        limitBox.Size = new Size(200, 35);
        limitBox.Minimum = 1; limitBox.Maximum = 50; limitBox.Value = 3;
        daysLabel = new Label();
        daysLabel.Name = "daysLabel";
        daysLabel.Location = new Point(25, 250);
        daysLabel.Size = new Size(450, 27);
        daysLabel.Text = "Số ngày mượn (ngày lịch)";
        daysLabel.AutoEllipsis = true;
        
        daysBox = new NumericUpDown();
        daysBox.Name = "daysBox";
        daysBox.Location = new Point(25, 285);
        daysBox.Size = new Size(200, 35);
        daysBox.Minimum = 1; daysBox.Maximum = 365; daysBox.Value = 14;
        overdueCheck = new CheckBox();
        overdueCheck.Name = "overdueCheck";
        overdueCheck.Location = new Point(25, 355);
        overdueCheck.Size = new Size(500, 28);
        overdueCheck.Text = "Chặn mượn mới khi có sách quá hạn";
        overdueCheck.Checked = true;
        loadButton = new Button();
        loadButton.Name = "loadButton";
        loadButton.Location = new Point(25, 420);
        loadButton.Size = new Size(200, 38);
        loadButton.Text = "Tải chính sách";
        loadButton.BackColor = Color.FromArgb(48, 78, 56); loadButton.ForeColor = Color.White; loadButton.FlatStyle = FlatStyle.Flat; loadButton.FlatAppearance.BorderSize = 0;
        loadButton.Click += Load_Click;
        saveButton = new Button();
        saveButton.Name = "saveButton";
        saveButton.Location = new Point(250, 420);
        saveButton.Size = new Size(240, 38);
        saveButton.Text = "Lưu phiên bản mới";
        saveButton.BackColor = Color.FromArgb(48, 78, 56); saveButton.ForeColor = Color.White; saveButton.FlatStyle = FlatStyle.Flat; saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += Save_Click;
        versionLabel = new Label();
        versionLabel.Name = "versionLabel";
        versionLabel.Location = new Point(25, 490);
        versionLabel.Size = new Size(1000, 27);
        versionLabel.Text = "Chưa tải phiên bản";
        versionLabel.AutoEllipsis = true;
        
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
        Name = "PolicyControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(hint);
        Controls.Add(limitLabel);
        Controls.Add(limitBox);
        Controls.Add(daysLabel);
        Controls.Add(daysBox);
        Controls.Add(overdueCheck);
        Controls.Add(loadButton);
        Controls.Add(saveButton);
        Controls.Add(versionLabel);
        Controls.Add(statusLabel);

        ResumeLayout(false);
        PerformLayout();
    }
}