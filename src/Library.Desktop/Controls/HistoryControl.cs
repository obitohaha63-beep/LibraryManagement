using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class HistoryControl : UserControl
{
    private LibraryApiClient? api;
    private int page = 1;
    private bool ready;

    public HistoryControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private void HistoryControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        UpdateFilters();
    }
    private void UpdateFilters()
    {
        if (!ready) return;
        page = 1;
        prevButton.Enabled = false;
        nextButton.Enabled = false;
        fromDate.Enabled = dateCheck.Checked;
        toDate.Enabled = dateCheck.Checked;
        searchButton.Enabled = !dateCheck.Checked || fromDate.Value.Date <= toDate.Value.Date;
        statusLabel.Text = searchButton.Enabled
            ? "Nhấn Tìm để áp dụng bộ lọc. Quá hạn chỉ tính những bản chưa trả."
            : "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.";
    }
    private void overdueCheck_CheckedChanged(object? sender, EventArgs e) => UpdateFilters();
    private void activeCheck_CheckedChanged(object? sender, EventArgs e) => UpdateFilters();
    private void dateCheck_CheckedChanged(object? sender, EventArgs e) => UpdateFilters();
    private void fromDate_ValueChanged(object? sender, EventArgs e) => UpdateFilters();
    private void toDate_ValueChanged(object? sender, EventArgs e) => UpdateFilters();

    private async Task LoadAsync()
    {
        if (dateCheck.Checked && fromDate.Value.Date > toDate.Value.Date)
            throw new InvalidOperationException("Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.");
        var path = $"api/loans?overdue={overdueCheck.Checked}&active={activeCheck.Checked}&page={page}";
        if (dateCheck.Checked) path += $"&from={fromDate.Value:yyyy-MM-dd}&to={toDate.Value:yyyy-MM-dd}";
        var rows = await Ui.RequireApi(api).GetAsync<Page<LoanDto>>(path);
        grid.DataSource = rows.Items;
        pageLabel.Text = $"Trang {page} · {rows.Total} chi tiết";
        prevButton.Enabled = page > 1;
        nextButton.Enabled = page * rows.Size < rows.Total;
    }
    private async void searchButton_Click(object? sender, EventArgs e)
    {
        page = 1;
        await Ui.RunAsync(this, LoadAsync, statusLabel);
    }
    private async void prevButton_Click(object? sender, EventArgs e)
    {
        page = Math.Max(1, page - 1);
        await Ui.RunAsync(this, LoadAsync, statusLabel);
    }
    private async void nextButton_Click(object? sender, EventArgs e)
    {
        page++;
        await Ui.RunAsync(this, LoadAsync, statusLabel);
    }
    private void grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not LoanDto loan) return;
        statusLabel.Text = $"Phiếu #{loan.LoanId} · {loan.CopyCode} · {loan.ReaderName} · {loan.Status} · Quá hạn {loan.DaysOverdue} ngày.";
    }
}
