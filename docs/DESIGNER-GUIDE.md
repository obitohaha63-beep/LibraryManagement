# Chỉnh giao diện và mở sự kiện bằng Visual Studio Designer

Mở `D:\thu_vien\ThuVien.sln`, build solution, chọn file `.cs` bên dưới rồi nhấn Shift+F7. Nếu Visual Studio đang giữ bản Design cũ, đóng tab Design, chấp nhận Reload khi có thông báo file thay đổi bên ngoài, rồi mở lại. Lưu thay đổi đang làm của bạn trước khi tải lại.

## Các màn hình

| File trong Library.Desktop | Double-click nền Designer | Các handler tiêu biểu |
|---|---|---|
| LoginForm.cs | LoginForm_Load | userBox_TextChanged, passwordBox_TextChanged, loginButton_Click |
| MainForm.cs | MainForm_Load | tabs_SelectedIndexChanged, circulationControl_Load và Load của các UserControl khác |
| Controls/CirculationControl.cs | CirculationControl_Load | readerSearch_TextChanged, readerCombo_SelectedIndexChanged, borrowButton_Click, returnButton_Click |
| Controls/CatalogControl.cs | CatalogControl_Load | titleBox_TextChanged, authorsList_SelectedIndexChanged, saveButton_Click, grid_CellContentClick |
| Controls/CopiesControl.cs | CopiesControl_Load | codeBox_TextChanged, stateCombo_SelectedIndexChanged, saveButton_Click |
| Controls/AccountsControl.cs | AccountsControl_Load | userBox_TextChanged, roleCombo_SelectedIndexChanged, filterRoleCombo_SelectedIndexChanged, saveButton_Click |
| Controls/HistoryControl.cs | HistoryControl_Load | dateCheck_CheckedChanged, fromDate_ValueChanged, searchButton_Click |
| Controls/MetadataControl.cs | MetadataControl_Load | kindCombo_SelectedIndexChanged, nameBox_TextChanged, saveButton_Click |
| Controls/PolicyControl.cs | PolicyControl_Load | limitBox_ValueChanged, daysBox_ValueChanged, overdueCheck_CheckedChanged, saveButton_Click |

## Thao tác đúng với UserControl

MainForm là cửa sổ chứa các UserControl. Trong MainForm Designer, double-click toàn bộ khối UserControl sẽ mở sự kiện Load của khối đó trong MainForm, nơi gắn kết nối API cho màn hình. Để sửa và double-click một ô nhập hoặc nút **bên trong** khối, mở Designer của chính file tương ứng trong thư mục `Controls`. Ví dụ muốn vào code nút Mượn: mở `CirculationControl.cs` → Shift+F7 → double-click nút Xác nhận mượn.

## Ý nghĩa sự kiện

- TextBox: TextChanged kiểm tra/hướng dẫn nhập liệu, đặt lại bộ lọc và trạng thái nút. Nhập liệu không tự gửi request mượn, trả hay lưu dữ liệu.
- Button: Click chạy chức năng tải/tìm/thêm mới/lưu/mượn/trả. Ô tìm kiếm còn có KeyDown để Enter thực hiện tìm.
- ComboBox: SelectedIndexChanged cập nhật lựa chọn hoặc xóa lựa chọn cũ khi đổi loại danh mục/vai trò.
- CheckBox: CheckedChanged cập nhật bộ lọc, hướng dẫn trạng thái hoặc xem trước chính sách.
- DateTimePicker/NumericUpDown: ValueChanged kiểm tra ngày hoặc cập nhật xem trước chính sách.
- DataGridView: CellContentClick mở xử lý chọn dòng; SelectionChanged giữ đồng bộ form khi dùng bàn phím hoặc tải dữ liệu.
- CheckedListBox tác giả: sự kiện mặc định SelectedIndexChanged hướng dẫn chọn tác giả; ItemCheck cập nhật số tác giả được đánh dấu.
- Form/UserControl: Load chuẩn bị trạng thái giao diện; constructor dùng cho Designer không truy cập mạng.

Trong Properties → biểu tượng tia sét, cột bên phải của các sự kiện trên đã có tên handler. Double-click tên handler để mở code hiện có. Nếu muốn xem sự kiện khác, chọn đúng dòng sự kiện trong bảng này. Nhãn và tiêu đề tĩnh chỉ trình bày nội dung, không có thao tác nghiệp vụ Click.

## Cách tổ chức mã

`.Designer.cs` chứa control, bố cục và gắn sự kiện, không có lambda hoặc gọi API trong InitializeComponent. `.resx` dành cho tài nguyên do Designer lưu. `.cs` chứa các handler có tên rõ ràng và helper giao diện. Nghiệp vụ mượn–trả, transaction và phân quyền vẫn ở service/API để Web và Desktop sử dụng chung.

Các file này được chuẩn hóa từ bản mã nguồn hiện có; không khẳng định bố cục đã được kéo thả lại thủ công. Kết quả cần đạt là có thể chỉnh, lưu và đi từ control trên Designer tới handler thật. Kết quả build và kiểm thử được ghi trong `TESTING.md`.
