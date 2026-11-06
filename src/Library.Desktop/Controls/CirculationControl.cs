using Library.Core;
using Library.Desktop.Services;
namespace Library.Desktop.Controls;
public partial class CirculationControl : UserControl
{
    private LibraryApiClient? api;
    private BorrowWorkflow? workflow;
    private bool ready;

    public CirculationControl() { InitializeComponent(); }

    public void Connect(LibraryApiClient client)
    {
        api = client;
        workflow = new BorrowWorkflow(client);
    }

    private void CirculationControl_Load(object? sender, EventArgs e)
    {
        ready = true;
        UpdateInputState();
    }

    private void readerSearch_TextChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        // A changed search must not leave the previous reader selected for a loan.
        readerCombo.DataSource = null;
        UpdateInputState();
        statusLabel.Text = "Nhấn Tìm bạn đọc, sau đó chọn đúng tài khoản cần mượn.";
    }

    private void readerCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        UpdateInputState();
        if (readerCombo.SelectedItem is UserDto reader)
            statusLabel.Text = $"Bạn đọc: {reader.UserName} · {reader.FullName} · {(reader.Active ? "Hoạt động" : "Đã khóa")}.";
    }

    private void codesBox_TextChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        UpdateInputState();
        statusLabel.Text = "Nhập mã bản sao cách nhau bằng dấu phẩy; server sẽ kiểm tra điều kiện mượn.";
    }

    private void returnBox_TextChanged(object? sender, EventArgs e)
    {
        if (!ready) return;
        UpdateInputState();
        statusLabel.Text = "Nhập mã của một bản sao rồi bấm Xác nhận trả.";
    }

    private void UpdateInputState()
    {
        findButton.Enabled = !string.IsNullOrWhiteSpace(readerSearch.Text);
        borrowButton.Enabled = readerCombo.SelectedItem is UserDto && !string.IsNullOrWhiteSpace(codesBox.Text);
        returnButton.Enabled = !string.IsNullOrWhiteSpace(returnBox.Text);
    }

    private void readerSearch_KeyDown(object? sender, KeyEventArgs e) => Ui.SearchOnEnter(e, findButton);

    private async void findButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            var client = Ui.RequireApi(api);
            var rows = await client.GetAsync<Page<UserDto>>("api/users?q=" + Uri.EscapeDataString(readerSearch.Text.Trim()));
            Ui.Bind(readerCombo, rows.Items, "UserName");
            if (rows.Total == 0) throw new InvalidOperationException("Không tìm thấy bạn đọc.");
        }, statusLabel);
        UpdateInputState();
    }

    private async void borrowButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            Ui.RequireApi(api);
            if (readerCombo.SelectedItem is not UserDto reader)
                throw new InvalidOperationException("Hãy tìm và chọn bạn đọc.");
            var id = await workflow!.BorrowAsync(reader.Id, codesBox.Text);
            codesBox.Clear();
            receiptLabel.Text = $"Đã tạo phiếu #{id}. Làm mới Website để đối chiếu.";
        }, statusLabel);
        UpdateInputState();
    }

    private async void returnButton_Click(object? sender, EventArgs e)
    {
        await Ui.RunAsync(this, async () =>
        {
            await Ui.RequireApi(api).SaveAsync("api/loans/return", new ReturnRequest(returnBox.Text.Trim()));
            returnBox.Clear();
        }, statusLabel);
        UpdateInputState();
    }
}
