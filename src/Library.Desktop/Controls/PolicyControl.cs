using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class PolicyControl : UserControl
{
    private LibraryApiClient? api;
    private bool ready;
    private bool fillingForm;

    public PolicyControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private void PolicyControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        statusLabel.Text = "Tải chính sách hiện tại trước khi thay đổi. Chỉ Manager được lưu.";
    }
    private void ShowPolicyHint()
    {
        if (!ready || fillingForm) return;
        statusLabel.Text = $"Chính sách mới: tối đa {limitBox.Value} bản, {daysBox.Value} ngày, {(overdueCheck.Checked ? "có" : "không")} chặn khi quá hạn. Bấm Lưu để tạo phiên bản.";
    }
    private void limitBox_ValueChanged(object? sender, EventArgs e) => ShowPolicyHint();
    private void daysBox_ValueChanged(object? sender, EventArgs e) => ShowPolicyHint();
    private void overdueCheck_CheckedChanged(object? sender, EventArgs e) => ShowPolicyHint();
    private async void loadButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            var policy = await Ui.RequireApi(api).GetAsync<PolicyVersion>("api/policy");
            fillingForm = true;
            try
            {
                limitBox.Value = policy.LoanLimit;
                daysBox.Value = policy.LoanDays;
                overdueCheck.Checked = policy.BlockIfOverdue;
                versionLabel.Text = $"Phiên bản #{policy.Id} · {policy.CreatedAt.AddHours(7):dd/MM/yyyy HH:mm}";
            }
            finally { fillingForm = false; }
        }, statusLabel);
    }
    private async void saveButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            await Ui.RequireApi(api).SaveAsync("api/policy", new PolicyRequest((int)limitBox.Value, (int)daysBox.Value, overdueCheck.Checked));
            versionLabel.Text = "Đã tạo phiên bản mới. Hạn trả của các phiếu cũ được giữ nguyên.";
        }, statusLabel);
    }
}
