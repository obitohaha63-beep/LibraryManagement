#nullable enable
namespace Library.Desktop.Controls;
partial class MetadataControl
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private Label heading = null!;
    private ComboBox kindCombo = null!;
    private Button loadButton = null!;
    private DataGridView grid = null!;
    private Label nameLabel = null!;
    private TextBox nameBox = null!;
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
        heading.Text = "DANH MỤC DÙNG CHUNG";
        heading.AutoEllipsis = true;
        
        kindCombo = new ComboBox();
        kindCombo.Name = "kindCombo";
        kindCombo.Location = new Point(25, 75);
        kindCombo.Size = new Size(300, 32);
        kindCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        kindCombo.Items.AddRange(new object[] { "Tác giả", "Thể loại", "Nhà xuất bản" }); kindCombo.SelectedIndex = 0; kindCombo.SelectedIndexChanged += Kind_Changed;
        loadButton = new Button();
        loadButton.Name = "loadButton";
        loadButton.Location = new Point(355, 72);
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
        nameLabel = new Label();
        nameLabel.Name = "nameLabel";
        nameLabel.Location = new Point(25, 430);
        nameLabel.Size = new Size(300, 27);
        nameLabel.Text = "Tên danh mục";
        nameLabel.AutoEllipsis = true;
        
        nameBox = new TextBox();
        nameBox.Name = "nameBox";
        nameBox.Location = new Point(25, 465);
        nameBox.Size = new Size(600, 31);
        
        newButton = new Button();
        newButton.Name = "newButton";
        newButton.Location = new Point(25, 535);
        newButton.Size = new Size(180, 38);
        newButton.Text = "Tạo mới";
        newButton.BackColor = Color.FromArgb(48, 78, 56); newButton.ForeColor = Color.White; newButton.FlatStyle = FlatStyle.Flat; newButton.FlatAppearance.BorderSize = 0;
        newButton.Click += New_Click;
        saveButton = new Button();
        saveButton.Name = "saveButton";
        saveButton.Location = new Point(230, 535);
        saveButton.Size = new Size(180, 38);
        saveButton.Text = "Lưu";
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
        Size = new Size(1080, 740);
        Name = "MetadataControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(kindCombo);
        Controls.Add(loadButton);
        Controls.Add(grid);
        Controls.Add(nameLabel);
        Controls.Add(nameBox);
        Controls.Add(newButton);
        Controls.Add(saveButton);
        Controls.Add(statusLabel);

        ResumeLayout(false);
        PerformLayout();
    }
}