#nullable enable
namespace Library.Desktop.Controls;
partial class AccountsControl
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
    private ComboBox filterRoleCombo = null!;
    private Button searchButton = null!;
    private DataGridView grid = null!;
    private Button prevButton = null!;
    private Button nextButton = null!;
    private Label pageLabel = null!;
    private Label userLabel = null!;
    private TextBox userBox = null!;
    private Label nameLabel = null!;
    private TextBox nameBox = null!;
    private Label passwordLabel = null!;
    private TextBox passwordBox = null!;
    private Label roleLabel = null!;
    private ComboBox roleCombo = null!;
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
        heading.Size = new Size(900, 27);
        heading.Text = "BẠN ĐỌC VÀ NHÂN VIÊN";
        heading.AutoEllipsis = true;
        
        searchBox = new TextBox();
        searchBox.Name = "searchBox";
        searchBox.Location = new Point(25, 75);
        searchBox.Size = new Size(280, 31);
        
        filterRoleCombo = new ComboBox();
        filterRoleCombo.Name = "filterRoleCombo";
        filterRoleCombo.Location = new Point(325, 75);
        filterRoleCombo.Size = new Size(200, 32);
        filterRoleCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        
        searchButton = new Button();
        searchButton.Name = "searchButton";
        searchButton.Location = new Point(545, 72);
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
        
        userLabel = new Label();
        userLabel.Name = "userLabel";
        userLabel.Location = new Point(25, 430);
        userLabel.Size = new Size(300, 27);
        userLabel.Text = "Tài khoản";
        userLabel.AutoEllipsis = true;
        
        userBox = new TextBox();
        userBox.Name = "userBox";
        userBox.Location = new Point(25, 465);
        userBox.Size = new Size(300, 31);
        
        nameLabel = new Label();
        nameLabel.Name = "nameLabel";
        nameLabel.Location = new Point(355, 430);
        nameLabel.Size = new Size(300, 27);
        nameLabel.Text = "Họ và tên";
        nameLabel.AutoEllipsis = true;
        
        nameBox = new TextBox();
        nameBox.Name = "nameBox";
        nameBox.Location = new Point(355, 465);
        nameBox.Size = new Size(300, 31);
        
        passwordLabel = new Label();
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Location = new Point(25, 520);
        passwordLabel.Size = new Size(300, 27);
        passwordLabel.Text = "Mật khẩu (chỉ khi tạo mới)";
        passwordLabel.AutoEllipsis = true;
        
        passwordBox = new TextBox();
        passwordBox.Name = "passwordBox";
        passwordBox.Location = new Point(25, 555);
        passwordBox.Size = new Size(300, 31);
        passwordBox.UseSystemPasswordChar = true;
        roleLabel = new Label();
        roleLabel.Name = "roleLabel";
        roleLabel.Location = new Point(355, 520);
        roleLabel.Size = new Size(300, 27);
        roleLabel.Text = "Vai trò";
        roleLabel.AutoEllipsis = true;
        
        roleCombo = new ComboBox();
        roleCombo.Name = "roleCombo";
        roleCombo.Location = new Point(355, 555);
        roleCombo.Size = new Size(300, 32);
        roleCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        
        activeCheck = new CheckBox();
        activeCheck.Name = "activeCheck";
        activeCheck.Location = new Point(25, 620);
        activeCheck.Size = new Size(350, 28);
        activeCheck.Text = "Tài khoản đang hoạt động";
        activeCheck.Checked = true;
        newButton = new Button();
        newButton.Name = "newButton";
        newButton.Location = new Point(700, 460);
        newButton.Size = new Size(220, 38);
        newButton.Text = "Tạo tài khoản";
        newButton.BackColor = Color.FromArgb(48, 78, 56);
        newButton.ForeColor = Color.White;
        newButton.FlatStyle = FlatStyle.Flat;
        newButton.FlatAppearance.BorderSize = 0;
        newButton.Click += newButton_Click;
        saveButton = new Button();
        saveButton.Name = "saveButton";
        saveButton.Location = new Point(700, 550);
        saveButton.Size = new Size(220, 38);
        saveButton.Text = "Lưu tài khoản";
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
        Name = "AccountsControl";
        Text = "Thư viện Đại học";
        AutoScroll = true;
        Controls.Add(heading);
        Controls.Add(searchBox);
        Controls.Add(filterRoleCombo);
        Controls.Add(searchButton);
        Controls.Add(grid);
        Controls.Add(prevButton);
        Controls.Add(nextButton);
        Controls.Add(pageLabel);
        Controls.Add(userLabel);
        Controls.Add(userBox);
        Controls.Add(nameLabel);
        Controls.Add(nameBox);
        Controls.Add(passwordLabel);
        Controls.Add(passwordBox);
        Controls.Add(roleLabel);
        Controls.Add(roleCombo);
        Controls.Add(activeCheck);
        Controls.Add(newButton);
        Controls.Add(saveButton);
        Controls.Add(statusLabel);

        Load += AccountsControl_Load;
        searchBox.TextChanged += searchBox_TextChanged;
        searchBox.KeyDown += searchBox_KeyDown;
        filterRoleCombo.SelectedIndexChanged += filterRoleCombo_SelectedIndexChanged;
        grid.CellContentClick += grid_CellContentClick;
        userBox.TextChanged += userBox_TextChanged;
        nameBox.TextChanged += nameBox_TextChanged;
        passwordBox.TextChanged += passwordBox_TextChanged;
        roleCombo.SelectedIndexChanged += roleCombo_SelectedIndexChanged;
        activeCheck.CheckedChanged += activeCheck_CheckedChanged;
        ResumeLayout(false);
        PerformLayout();
    }
}