namespace Library.Desktop;
internal static class Ui
{
    public static async Task RunAsync(Control control, Func<Task> action, Label status)
    {
        control.Enabled = false; status.Text = "Đang xử lý…";
        try { await action(); status.Text = "Hoàn tất. Dữ liệu đã được cập nhật."; }
        catch (Exception ex) { status.Text = ex is HttpRequestException ? "Không kết nối được server. Kiểm tra địa chỉ và khởi động Server." : ex.Message; }
        finally { control.Enabled = true; }
    }
    public static void Bind<T>(ComboBox combo, List<T> rows, string display = "Name", string value = "Id")
    { combo.DataSource = rows; combo.DisplayMember = display; combo.ValueMember = value; }
    public static int Int(TextBox box) => int.TryParse(box.Text, out var n) ? n : 0;
}
