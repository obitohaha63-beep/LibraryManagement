#nullable enable
namespace Library.Desktop;
partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private Label welcomeLabel = null!;
    private TabControl tabs = null!;
    private TabPage circulationTab = null!;
    private Controls.CirculationControl circulationControl = null!;
    private TabPage catalogTab = null!;
    private Controls.CatalogControl catalogControl = null!;
    private TabPage copiesTab = null!;
    private Controls.CopiesControl copiesControl = null!;
    private TabPage accountsTab = null!;
    private Controls.AccountsControl accountsControl = null!;
    private TabPage historyTab = null!;
    private Controls.HistoryControl historyControl = null!;
    private TabPage metadataTab = null!;
    private Controls.MetadataControl metadataControl = null!;
    private TabPage policyTab = null!;
    private Controls.PolicyControl policyControl = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        welcomeLabel = new Label();
        welcomeLabel.Name = "welcomeLabel";
        welcomeLabel.Location = new Point(25, 20);
        welcomeLabel.Size = new Size(1000, 27);
        welcomeLabel.Text = "THƯ VIỆN ĐẠI HỌC · BÀN LÀM VIỆC THỦ THƯ";
        welcomeLabel.AutoEllipsis = true;
        
        tabs = new TabControl();
        tabs.Name = "tabs";
        tabs.Location = new Point(18, 65);
        tabs.Size = new Size(1110, 795);
        tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        circulationTab = new TabPage("Mượn / Trả"); circulationTab.Name = "circulationTab";
        circulationControl = new Controls.CirculationControl(); circulationControl.Dock = DockStyle.Fill;
        circulationTab.Controls.Add(circulationControl); tabs.TabPages.Add(circulationTab);
        catalogTab = new TabPage("Đầu sách"); catalogTab.Name = "catalogTab";
        catalogControl = new Controls.CatalogControl(); catalogControl.Dock = DockStyle.Fill;
        catalogTab.Controls.Add(catalogControl); tabs.TabPages.Add(catalogTab);
        copiesTab = new TabPage("Bản sao"); copiesTab.Name = "copiesTab";
        copiesControl = new Controls.CopiesControl(); copiesControl.Dock = DockStyle.Fill;
        copiesTab.Controls.Add(copiesControl); tabs.TabPages.Add(copiesTab);
        accountsTab = new TabPage("Tài khoản"); accountsTab.Name = "accountsTab";
        accountsControl = new Controls.AccountsControl(); accountsControl.Dock = DockStyle.Fill;
        accountsTab.Controls.Add(accountsControl); tabs.TabPages.Add(accountsTab);
        historyTab = new TabPage("Lịch sử / Quá hạn"); historyTab.Name = "historyTab";
        historyControl = new Controls.HistoryControl(); historyControl.Dock = DockStyle.Fill;
        historyTab.Controls.Add(historyControl); tabs.TabPages.Add(historyTab);
        metadataTab = new TabPage("Danh mục"); metadataTab.Name = "metadataTab";
        metadataControl = new Controls.MetadataControl(); metadataControl.Dock = DockStyle.Fill;
        metadataTab.Controls.Add(metadataControl); tabs.TabPages.Add(metadataTab);
        policyTab = new TabPage("Chính sách"); policyTab.Name = "policyTab";
        policyControl = new Controls.PolicyControl(); policyControl.Dock = DockStyle.Fill;
        policyTab.Controls.Add(policyControl); tabs.TabPages.Add(policyTab);

        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(248, 248, 242);
        ForeColor = Color.FromArgb(36, 56, 47);
        ClientSize = new Size(1150, 890);
        Name = "MainForm";
        Text = "Thư viện Đại học";
        StartPosition = FormStartPosition.CenterScreen; MinimumSize = new Size(900, 760);
        Controls.Add(welcomeLabel);
        Controls.Add(tabs);

        ResumeLayout(false);
        PerformLayout();
    }
}