# Thư viện Đại học

Bản POC chạy được với C# 13, .NET 9, ASP.NET Core MVC/Web API, Razor Views, Bootstrap 5, WinForms, EF Core 9, ASP.NET Core Identity và SQL Server. Web dành cho bạn đọc, WinForms dành cho thủ thư/quản lý. Cả hai sử dụng cùng dữ liệu và cùng nghiệp vụ mượn–trả ở server.

## Chạy nhanh

Yêu cầu Windows, .NET SDK 9 và SQL Server Express. Máy đang dùng đã có SDK 9.0.318 và instance `SQLEXPRESS`.

Mở PowerShell tại thư mục dự án:

```powershell
./scripts/Start-Poc.ps1
```

Script build solution, chạy Web/API nền tại **http://localhost:5080** và mở WinForms. Chỉ mở Web: `./scripts/Start-Poc.ps1 -WebOnly`. Sau khi đã build có thể thêm `-SkipBuild`.

Hoặc chạy thủ công trong hai terminal:

```powershell
dotnet run --project src/Library.Server --launch-profile http
dotnet run --project src/Library.Desktop
```

Server tự áp dụng migration và tạo dữ liệu mẫu nếu chưa có. Cơ sở dữ liệu demo: `LibraryUniversityPoc20261023`. Không dùng lại database đã có của một dự án khác. Không có thao tác tự xóa hoặc reset database demo.

Nếu SQL Server đang dừng, mở PowerShell **Run as administrator** và chạy:

```powershell
Start-Service -Name 'MSSQL$SQLEXPRESS'
```

Nếu dùng instance khác, đặt `ConnectionStrings__Library` trước khi chạy server, hoặc tạo `src/Library.Server/appsettings.Local.json` với khóa `ConnectionStrings:Library`. File Local được loại khỏi Git. Cấu hình mẫu dùng Windows Authentication, không chứa mật khẩu SQL.

## Tài khoản demo

Mật khẩu chung: **Library@123**. Đây là dữ liệu thử nghiệm, chỉ dùng trong môi trường POC cục bộ.

| Tài khoản | Vai trò | Sử dụng |
|---|---|---|
| manager | Manager | WinForms, quản trị nhân viên và chính sách |
| librarian | Librarian | WinForms, danh mục/bản sao/bạn đọc/mượn–trả |
| SV001 | Reader | Website, sách đang mượn và lịch sử cá nhân |
| SV002 | Reader | Website, kiểm tra phân tách dữ liệu |

Trong màn hình WinForms, địa chỉ mặc định là `http://localhost:5080`. Web và API cùng một host để việc chạy đồ án đơn giản. Web gọi trực tiếp application service trong host; WinForms gọi API qua HTTP. Browser không kết nối SQL Server.

## Kịch bản demo end-to-end

1. Mở Web, tìm **Lập trình C#**, xem số bản có sẵn.
2. Đăng nhập WinForms bằng `librarian`. Tab **Mượn / Trả**, tìm `SV001`, chọn tài khoản.
3. Nhập `CP001, CP002`, xác nhận mượn. Màn hình hiển thị mã phiếu vừa tạo.
4. Đăng nhập Web bằng `SV001`. Trang **Sách của tôi** hiển thị hai bản sao và hạn trả; mở mã phiếu để xem chi tiết. Tải lại trang sách để thấy số khả dụng giảm.
5. Cố mượn `CP001` lần nữa: server từ chối, không tạo phiếu mới.
6. Trả `CP001` bằng WinForms. Web còn `CP002` đang mượn; lịch sử giữ cả bản đã trả và bản đang mượn.
7. Trả `CP001` lần nữa: bị từ chối. Trả nốt `CP002` để hoàn tất demo.
8. Tab **Tài khoản** cho phép khóa một bạn đọc; thử mượn mới để thấy bị chặn. Manager có tab **Chính sách**; Librarian không có tab này và API vẫn từ chối nếu gọi trực tiếp.

Tab **Đầu sách**: tải danh mục trước, chọn dòng để sửa hoặc bấm Tạo mới; hỗ trợ nhiều tác giả. Bỏ chọn “Đang hiển thị” để ngừng phục vụ, không xóa lịch sử. Tab **Bản sao**: nhập ID đầu sách xem ở bảng Đầu sách; mã bản sao không thay đổi sau tạo. Tab **Danh mục** quản lý tác giả/thể loại/NXB. Tab **Tài khoản** có bộ lọc vai trò; nhân viên chỉ do Manager quản lý.

## WinForms Designer

Mở `ThuVien.sln` trong Visual Studio có workload **.NET desktop development** và **ASP.NET and web development**. Build một lần, mở `LoginForm.cs`, `MainForm.cs` hoặc các file trong `Controls`, bấm **Shift+F7 / View Designer**.

Toàn bộ 9 form/UserControl có bộ file `.cs`, `.Designer.cs`, `.resx` và metadata Form/UserControl trong project. Constructor không tham số chỉ tạo giao diện, không gọi API/DB; dùng được khi server chưa chạy. Bố cục và gắn sự kiện được quản lý trong Designer; script sinh bố cục cũ đã được bỏ. Chỉnh vị trí, kích thước, thuộc tính và sự kiện trực tiếp bằng Visual Studio Designer rồi lưu bình thường.

Trong `LoginForm.cs [Design]`, double-click ô tài khoản mở `userBox_TextChanged`, ô mật khẩu mở `passwordBox_TextChanged`, ô server mở `urlBox_TextChanged`; các sự kiện này kiểm tra dữ liệu và cập nhật nút đăng nhập. Double-click nút Đăng nhập mở `loginButton_Click`, nơi gọi API xác thực. Double-click nền form mở `LoginForm_Load`. Có thể kiểm tra hoặc chọn lại handler trong **Properties → biểu tượng tia sét (Events)**. `TextChanged` xử lý thay đổi nội dung; không phải sự kiện đăng nhập.

Cách làm này áp dụng cho tất cả 7 màn hình nghiệp vụ: nút mở `tenNut_Click`, ô nhập mở `tenO_TextChanged`, combobox mở `tenCombo_SelectedIndexChanged`, checkbox mở `tenCheck_CheckedChanged`, bảng mở `grid_CellContentClick`, bộ chọn ngày/số mở `ValueChanged`. Xem [hướng dẫn Designer và danh sách sự kiện](docs/DESIGNER-GUIDE.md).

## Tách lớp và cách đọc code

| Vị trí | Trách nhiệm |
|---|---|
| Library.Core | Entity, DTO, DataAnnotations, quy tắc thuần và ngày nghiệp vụ |
| Library.Server/Services | Nghiệp vụ, truy vấn, transaction; dùng chung cho Web/API |
| Library.Server/Data | EF DbContext, Identity, mapping, migration và seed |
| Library.Server/Controllers | Nhận request, kiểm tra role, gọi service, trả HTTP/View |
| Library.Server/Views | Razor chỉ trình bày dữ liệu; bảng lịch sử dùng partial chung |
| Library.Desktop/Controls | Giao diện và xử lý sự kiện; không chứa SQL/quy tắc hạn mức |
| Library.Desktop/Services | HTTP client chung, validation và vòng đời mã xác nhận mượn |
| Library.Tests | Unit test và integration test bằng SQL Server thật |
| Library.Desktop.Smoke | Kiểm tra Designer và luồng nút WinForms qua HTTP thật |

Thiết kế cố ý dùng ít project, DI constructor và EF Core trực tiếp trong service, không thêm CQRS/MediatR hay repository chỉ bọc lại DbSet. Sinh viên nên đọc `LoanRules` → `CirculationService` → `LoansApiController` → `CirculationControl`.

POC đạt 32 kiểm thử trước bước tách thêm quản trị khỏi controller. Bản cuối đưa tài khoản/chính sách/danh mục vào `AdministrationService`, bản sao vào `CopyService`; giao diện dùng `LibraryApiClient` và `Ui` để giảm lặp; mượn/trả chỉ có một cách xử lý tại `CirculationService`. Bộ hồi quy cuối và kết quả nằm trong `docs/TESTING.md`.

## Dữ liệu, giao dịch và quyền

- Mượn và trả nằm trong transaction SQL Server. `sp_getapplock` khóa nghiệp vụ trong transaction, bảo vệ cả bản sao lẫn tổng hạn mức của bạn đọc. Với quy mô POC, các giao dịch lưu thông được thực hiện lần lượt; đây là đánh đổi dễ đọc, dễ chứng minh tính đúng.
- Unique filtered index chặn hai `LoanItem ON_LOAN` cho cùng một bản sao. Unique index bảo vệ ISBN, CopyCode và cặp staff/requestKey. Không giữ biến đếm khả dụng riêng.
- Gửi lại cùng requestKey và cùng nội dung trả về cùng phiếu. Tái sử dụng key cho nội dung khác bị từ chối. WinForms giữ key qua lỗi mạng để retry an toàn.
- Trả từng bản lưu thời gian/người trả, không thay đổi các bản còn mượn trong phiếu.
- Policy chỉ thêm phiên bản mới. Loan lưu policy ID và hạn trả từng chi tiết; thay chính sách không sửa hạn cũ.
- Ngày nghiệp vụ theo Việt Nam UTC+7; thời điểm giao dịch lưu UTC. Hạn hôm nay chưa tính quá hạn.
- Mật khẩu do ASP.NET Core Identity băm; đăng nhập sai có lockout. Desktop dùng opaque bearer token của Identity, Web dùng cookie HttpOnly và antiforgery. Không tự đăng ký tài khoản.
- Server đọc lại Active/role mỗi request để khóa tài khoản/hạ quyền có hiệu lực với phiên đã đăng nhập. Reader luôn lấy ID từ phiên, không dùng ID do client cung cấp.

## Style và ảnh sách

`wwwroot/css/root.css` sao chép từ file bạn cung cấp. Web dùng palette nền kem, xanh rừng/sage, serif cho tiêu đề; WinForms dùng cùng màu nền/chữ/nút, control chuẩn để dễ sửa bằng Designer. Bootstrap 5.3.3 được lưu cục bộ kèm license MIT. Google Fonts có fallback khi offline.

Database chỉ lưu đường dẫn như `images/books/book-1.svg`. Ảnh nằm dưới `src/Library.Server/wwwroot/images/books`; Web tạo URL từ đường dẫn đó. Server từ chối ổ đĩa tuyệt đối, URL ngoài, dấu `..` và backslash. Bìa demo là SVG có sẵn, không cần đường dẫn máy cá nhân. POC nhập đường dẫn file đã đặt vào thư mục ảnh, chưa có chức năng upload.

## Kiểm thử

```powershell
./scripts/Test-Poc.ps1
# Sau khi server POC đã chạy, có thể chạy thêm WinForms:
./scripts/Test-Poc.ps1 -IncludeDesktop
```

Integration test tạo database riêng `LibraryPoc_Test_<GUID>`, áp migration/seed và xóa đúng database test đó khi kết thúc; không đụng dữ liệu demo. Tài khoản Windows cần quyền tạo/xóa database test. Có thể đổi instance bằng biến `LIBRARY_TEST_SERVER`. Không thay SQL Server bằng EF InMemory/SQLite cho bài kiểm tra transaction/concurrency.

Kiểm tra riêng Designer không cần server:

```powershell
dotnet run --project tests/Library.Desktop.Smoke -- artifacts/screenshots --offline
```

Kết quả test TRX ở `artifacts/TestResults`, ảnh các tab Designer ở `artifacts/screenshots`. Xem thêm `docs/TESTING.md` và `docs/PRD-TRACEABILITY.md`.

## Migration và OpenAPI

```powershell
dotnet tool restore
dotnet ef database update --project src/Library.Server
```

OpenAPI JSON: **http://localhost:5080/openapi/v1.json**. `GET /api/books` và metadata công khai; `POST /api/auth/login` trả accessToken để dùng `Authorization: Bearer ...`; `/api/loans/mine` chỉ Reader; mượn/trả, catalog mutation, copies/users/metadata chỉ Staff; `/api/policy` chỉ Manager. DTO nằm trong `Library.Core/Contracts.cs`.

HTTP dùng cho demo localhost. Có profile HTTPS tại `https://localhost:7080` (`--launch-profile https`); chỉ dùng chứng chỉ dev đã được tin cậy. Khi triển khai thực cần cấu hình HTTPS, tài khoản/bí mật thật và thay quy trình seed demo.

## Phạm vi và giả định

P0 triển khai theo PRD. Các quy tắc 3 bản, 14 ngày, chặn khi quá hạn và không hồi tố là **giả định thử nghiệm**, chưa phải chính sách thư viện được phê duyệt. POC cho Librarian khóa/mở Reader theo FR-12; quyết định này cần đối chiếu Q-12 trước nghiệm thu thực tế. ISBN bắt buộc duy nhất, dạng 10–20 ký tự số/X/dấu gạch; chưa kiểm tra checksum ISBN.

P1 đặt trước, gia hạn, mất/hỏng, phí phạt, thông báo và dashboard nâng cao chưa triển khai. Không có cam kết hiệu năng nhiều quầy cho khóa lưu thông toàn cục. Bản POC có mã nguồn và kiểm thử; việc giảng viên duyệt chính sách và nghiệm thu toàn bộ NFR vẫn cần thực hiện riêng.
