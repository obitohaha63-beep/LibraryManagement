using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class PolicyControl : UserControl
{
    private LibraryApiClient? api;
    public PolicyControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private async void Load_Click(object? s, EventArgs e) => await Ui.RunAsync(loadButton, async () => { var p = await api!.GetAsync<PolicyVersion>("api/policy"); limitBox.Value = p.LoanLimit; daysBox.Value = p.LoanDays; overdueCheck.Checked = p.BlockIfOverdue; versionLabel.Text = $"Phiên bản #{p.Id} · {p.CreatedAt.AddHours(7):dd/MM/yyyy HH:mm}"; }, statusLabel);
    private async void Save_Click(object? s, EventArgs e) => await Ui.RunAsync(saveButton, async () => { await api!.SaveAsync("api/policy", new PolicyRequest((int)limitBox.Value, (int)daysBox.Value, overdueCheck.Checked)); versionLabel.Text = "Đã tạo phiên bản mới. Hạn trả của các phiếu cũ được giữ nguyên."; }, statusLabel);
}
