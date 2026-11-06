# Kiểm thử và xác minh POC

Kiểm tra ngày **23/10/2026**, Windows, .NET SDK 9.0.318, SQL Server Express 2022. Database test riêng theo GUID, có migration thật và unique filtered index; không dùng InMemory provider.

## Kết quả

- Build toàn solution: **0 lỗi, 0 cảnh báo**.
- Trước khi tách thêm quản trị khỏi controller: **32/32** unit/integration test đạt.
- Sau refactor, bổ sung thu hồi quyền, quản trị và đăng nhập cookie: **39/39** đạt.
- Bản cuối có thêm kiểm tra IDOR chi tiết phiếu và kiểm tra riêng ISBN/ảnh: **41/41** đạt, **0 bỏ qua**. Kết quả TRX tại `artifacts/TestResults/library-tests.trx`.
- Smoke WinForms: tất cả **7 tab** khởi tạo được khi chưa có server; luồng login → tìm Reader → mượn → catalog chung → trả → lịch sử đạt qua HTTP/SQL Server thật.
- Giao diện Web được mở trên trình duyệt, kiểm tra palette và bố cục responsive; ảnh WinForms trong `artifacts/screenshots` được xuất từ control thật để kiểm tra bố cục.

## Chạy lại

```powershell
dotnet build ThuVien.sln
./scripts/Test-Poc.ps1
```

Khi Web/API localhost:5080 đã chạy:

```powershell
dotnet run --project tests/Library.Desktop.Smoke -- artifacts/screenshots
```

Hoặc chỉ kiểm tra khả năng khởi tạo Designer:

```powershell
dotnet run --project tests/Library.Desktop.Smoke -- artifacts/screenshots --offline
```

Smoke online tạo Reader và một bản sao thử nghiệm riêng, mượn/trả thành công rồi chuyển Reader sang không hoạt động, bản sao sang INACTIVE; lịch sử được giữ. Không dùng CP001/CP002 của kịch bản demo.

## Kiểm tra quan trọng

Unit test kiểm tra ngày lịch, hạn hôm nay, ngày tương lai, bản đã trả, mã bản sao trùng/normalize, biên hạn mức và đường dẫn ảnh tương đối.

Integration test kiểm tra một phiếu nhiều bản; hai thủ thư cùng mượn một copy chỉ một request thành công; hai giao dịch cùng Reader không vượt hạn mức; request retry trả cùng ID; request key bị đổi nội dung bị từ chối; trả lần hai; sách ẩn không được mượn mới; token bị khóa/hạ role; Reader không xem phiếu của người khác; ngày lọc, phân trang 13 sách, chỉnh policy và antiforgery Web.

Fault injection dùng `SaveChangesInterceptor`: phát sinh lỗi **sau khi SQL đã ghi nhưng trước commit**. Sau response lỗi, một DbContext mới đọc lại để xác nhận Loan/LoanItem/Copy đều rollback. Kiểm tra này mạnh hơn việc chỉ thử mã copy sai trước khi lưu. Có test chèn trực tiếp LoanItem trùng để chứng minh DB index vẫn chặn khi bỏ qua service.

## Phần cần kiểm tra thủ công

Mở Visual Studio Designer bằng Shift+F7, kéo/chỉnh control, dùng bàn phím, thử DPI khác, chạy màn hình nhỏ, đổi instance SQL và duyệt chính sách nghiệp vụ. Smoke tạo control cùng constructor như Designer nhưng không giả định thay thế hoàn toàn việc mở Visual Studio Designer. Chưa thực hiện kiểm thử tải hoặc UAT của giảng viên.

Tham khảo kỹ thuật: [EF Core transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions) và [EF Core concurrency](https://learn.microsoft.com/en-us/ef/core/saving/concurrency). Khóa SQL và index trong dự án được kiểm chứng bằng SQL Server integration test.

## Cập nhật LoginForm ngày 29/10/2026

Gắn trực tiếp các sự kiện `LoginForm_Load`, `userBox_TextChanged`, `passwordBox_TextChanged`, `urlBox_TextChanged` và `loginButton_Click` trong Designer. Ô nhập kiểm tra dữ liệu và trạng thái nút; chỉ Click/Enter mới gọi API. Đồng thời sửa HTTP client để có thể thử đăng nhập lại sau một lần thất bại mà không gán lại BaseAddress của client đã gửi request.

Build Desktop và Desktop.Smoke đạt 0 lỗi, 0 cảnh báo. Smoke offline đạt: khởi tạo các tab không cần server; LoginForm phản hồi khi tài khoản/mật khẩu trống, khi địa chỉ server sai và khi sửa lại dữ liệu hợp lệ. Đã bổ sung kịch bản đăng nhập sai rồi đăng nhập lại vào smoke online, nhưng chưa chạy lại kịch bản online trong lượt này vì Web/API và dịch vụ SQL Server đang dừng. Kết quả 41/41 phía trên là kết quả đã lưu của phiên bản trước, không phải lần chạy lại ngày 29/10.

## Chuẩn hóa toàn bộ Designer ngày 29/10/2026

Sau yêu cầu áp dụng cho toàn dự án WinForms, đã chuẩn hóa cả **9 form/UserControl** và **98 kết nối sự kiện** trong Designer. Mỗi màn hình có `.cs`, `.Designer.cs`, `.resx`; project chứa metadata để Visual Studio nhận diện/nest các file. Đã bỏ script sinh bố cục cũ. Handler dùng tên control và tên sự kiện; không tạo control động trong code-behind hoặc gọi API trong InitializeComponent.

Kết quả kiểm tra lại sau lần chuẩn hóa này:

- Build toàn solution: **0 lỗi, 0 cảnh báo**.
- Unit/integration test SQL Server: **41/41 đạt**, 0 bỏ qua; TRX được cập nhật tại `artifacts/TestResults/library-tests.trx`.
- Kiểm tra đầu vào LoginForm, ngày lọc lịch sử, chọn sách/bản sao/tài khoản, tạo mới, đổi loại danh mục, xem trước chính sách: đạt offline.
- Đăng nhập sai rồi thử lại, mượn/trả và đọc catalog/lịch sử chung qua API/SQL Server thật: đạt.
- Tải dữ liệu từ các tab Đầu sách, Bản sao, Tài khoản, Lịch sử, Danh mục và Chính sách bằng handler Click/Load mới: đạt.
- Librarian không có tab chính sách; Manager tải được chính sách: đạt.
- Rà soát sự kiện: 98 kết nối đều có handler tương ứng; đủ 9 file resx.

Đã tìm thấy và xem được Visual Studio đang mở LoginForm Designer. Người dùng dừng Computer Use bằng Escape trước bước đóng/mở lại Designer và double-click kiểm chứng, nên **chưa xác nhận trực tiếp thao tác điều hướng double-click trong Visual Studio sau sửa**. Các kết quả tự động ở trên kiểm tra control và handler thật, không thay thế bước kiểm tra điều hướng của IDE. Xem `DESIGNER-GUIDE.md` để mở đúng UserControl và handler.
