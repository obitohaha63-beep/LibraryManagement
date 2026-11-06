using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class CirculationControl : UserControl
{
    private LibraryApiClient? api; private BorrowWorkflow? workflow;
    public CirculationControl() { InitializeComponent(); }
    public void Connect(LibraryApiClient client) { api = client; workflow = new(client); }
    private async void Find_Click(object? sender, EventArgs e) => await Ui.RunAsync(findButton, async () => {
        var rows = await api!.GetAsync<Page<UserDto>>("api/users?q=" + Uri.EscapeDataString(readerSearch.Text));
        Ui.Bind(readerCombo, rows.Items, "UserName"); if (rows.Total == 0) throw new InvalidOperationException("Không tìm thấy bạn đọc."); }, statusLabel);
    private async void Borrow_Click(object? sender, EventArgs e) => await Ui.RunAsync(borrowButton, async () => {
        if (readerCombo.SelectedItem is not UserDto reader) throw new InvalidOperationException("Hãy tìm và chọn bạn đọc.");
        var id = await workflow!.BorrowAsync(reader.Id, codesBox.Text); codesBox.Clear(); receiptLabel.Text = $"Đã tạo phiếu #{id}. Làm mới Website để đối chiếu."; }, statusLabel);
    private async void Return_Click(object? sender, EventArgs e) => await Ui.RunAsync(returnButton, async () => { await api!.SaveAsync("api/loans/return", new ReturnRequest(returnBox.Text.Trim())); returnBox.Clear(); }, statusLabel);
}
