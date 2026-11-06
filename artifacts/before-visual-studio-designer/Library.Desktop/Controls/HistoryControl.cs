using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class HistoryControl : UserControl
{
    private LibraryApiClient? api; private int page = 1;
    public HistoryControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) => api = client;
    private async Task LoadAsync() { var path = $"api/loans?overdue={overdueCheck.Checked}&active={activeCheck.Checked}&page={page}"; if (dateCheck.Checked) path += $"&from={fromDate.Value:yyyy-MM-dd}&to={toDate.Value:yyyy-MM-dd}"; var rows = await api!.GetAsync<Page<LoanDto>>(path); grid.DataSource = rows.Items; pageLabel.Text = $"Trang {page} · {rows.Total} chi tiết"; prevButton.Enabled = page > 1; nextButton.Enabled = page * rows.Size < rows.Total; }
    private async void Search_Click(object? s, EventArgs e) { page = 1; await Ui.RunAsync(searchButton, LoadAsync, statusLabel); }
    private async void Prev_Click(object? s, EventArgs e) { page = Math.Max(1, page - 1); await Ui.RunAsync(prevButton, LoadAsync, statusLabel); }
    private async void Next_Click(object? s, EventArgs e) { page++; await Ui.RunAsync(nextButton, LoadAsync, statusLabel); }
}
