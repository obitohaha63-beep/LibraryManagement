# Đối chiếu PRD và bản POC

Nguồn yêu cầu: `PRD_Quan_Ly_Thu_Vien_Ban_Giao_Dev_v1.0.docx`, ngày 23/10/2026. Stack theo ảnh người dùng, giao diện theo `root.css`. Nội dung PRD được dùng làm đặc tả sản phẩm; các chỉ dẫn quy trình trong tài liệu không được coi là quyền thao tác ngoài yêu cầu của người dùng.

## P0

| FR | Chức năng đã triển khai | Nơi kiểm chứng |
|---|---|---|
| 01–02 | Tìm tên/tác giả/ISBN, collation CI, lọc thể loại/tác giả/còn sách, phân trang | CatalogService, Web Catalog, AC01_02_07 và SearchPaginatesMoreThanOnePage |
| 03 | Chi tiết đầu sách, tổng/khả dụng tính từ BookCopy | Catalog Details, AC03_11_14, Desktop smoke |
| 04–05 | Identity, hash, lockout, Active, RBAC, chống IDOR | AuthApiController/Web Account, AC04_05_18, RevokedRole, ReaderCanOpenOwnLoan |
| 06–07 | Đang mượn, quá hạn, lịch sử lọc ngày/phân trang, chi tiết phiếu cá nhân | MyLoans và _LoanTable, AC06_16, AC01_02_07, CookieLoginProtectsPosts |
| 08 | Thêm/sửa/ẩn đầu sách, validate, ISBN duy nhất | CatalogControl/CatalogService, HiddenBook, DuplicateIsbnAndAbsoluteImages |
| 09 | Tác giả, thể loại, NXB, nhiều tác giả trên sách | MetadataControl, AdministrationService, MetadataCanBeCreatedEdited |
| 10–11 | Tạo bản sao, mã duy nhất, sửa vị trí/AVAILABLE/INACTIVE, ON_LOAN qua transaction | CopiesControl/CopyService, AC08_09, CopyStateCannotBypassLoanTransactions |
| 12 | Cấp/sửa/khóa Reader, bảo toàn lịch sử | AccountsControl/AdministrationService, AC04_05_18, HiddenBook |
| 13–15 | Phiếu nhiều bản, hạn mức/quá hạn, transaction, requestKey, chống mượn kép | CirculationService, AC11–13/19/20, database index test, rollback test |
| 16–17 | Trả từng bản, tìm active item bằng CopyCode, từ chối trả lặp, lưu người/thời điểm trả | CirculationControl, AC14–15, Desktop smoke |
| 18 | Lịch sử toàn thư viện, lọc đang mượn/quá hạn/ngày, phân trang | HistoryControl, AC06_16/AC01_02_07 |
| 19 | Manager thêm policy version; không hồi tố | PolicyControl, AC17_PolicyVersionDoesNotChangeSavedDueDates |
| 20 | Manager cấp/sửa vai trò/Active nhân viên | AccountsControl, LibrarianCannotCreateOrEditStaff, RevokedRole |

## Điều kiện nghiệm thu

AC-01..20 được đối chiếu bằng các kiểm thử trong `Library.Tests`, smoke test WinForms và kịch bản demo trong README. Không coi một test mang nhiều mã AC là bằng chứng đã bao phủ mọi tổ hợp dữ liệu: bộ kiểm thử tập trung happy path, biên, giao dịch, concurrency, rollback và quyền. Các trạng thái UI/khả năng sử dụng vẫn cần kiểm tra thủ công trong Visual Studio và trên thiết bị trình diễn.

| Nhóm | Bằng chứng tự động |
|---|---|
| AC-01..03 | Tìm case-insensitive, 13 đầu sách phân trang 12+1, đếm bản sao sau mượn/trả |
| AC-04..07 | Sai mật khẩu, khóa token, IDOR danh sách/chi tiết, ngày lọc sai, quá hạn hai ngày |
| AC-08..10 | Validation đầu sách, ISBN/copy trùng, ẩn sách giữ lịch sử, khóa Reader |
| AC-11..13 | DueDate, phiếu nhiều bản, hạn mức, hai request cùng copy, hai request cùng Reader |
| AC-14..16 | Trả từng bản, trả lặp, fault injection sau SQL write/trước commit, days overdue |
| AC-17..18 | Policy version, hạn cũ không đổi, Librarian không được quản lý policy/nhân viên |
| AC-19..20 | Rollback toàn phiếu, fault injection mượn, retry đồng thời cùng key, key khác nội dung |

## Giả định cần xác nhận

3 bản đang mượn, 14 ngày lịch, chặn khi quá hạn, không hồi tố chính sách. Librarian được đổi trạng thái Reader theo FR-12 để POC hoạt động; Q-12 vẫn cần người có thẩm quyền chốt trước nghiệm thu thực tế. ISBN áp dụng duy nhất và validation định dạng, chưa kiểm checksum.

## P1 chưa thực hiện

FR-21..23 / AC-21..23: đặt trước, gia hạn, mất/hỏng, phí phạt, thông báo, dashboard nâng cao. Không mở rộng P1 khi P0 chưa được người dùng nghiệm thu.

## Giới hạn kỹ thuật POC

Các service và EF Data tách theo folder trong một server project để dễ học và debug. Web MVC và API cùng host, không cần chạy thêm một Web frontend server riêng. Khóa giao dịch lưu thông toàn cục ưu tiên tính đúng; chưa đo tải nhiều quầy. Designer dùng control WinForms chuẩn, palette đồng nhất CSS; không mô phỏng toàn bộ border-radius/font web. Ảnh được chọn bằng đường dẫn file tương đối, chưa upload.

Kiểm tra deployment thực tế, backup/restore, hiệu năng định lượng, khả năng truy cập toàn diện, chính sách mật khẩu thật và UAT giảng viên nằm ngoài bằng chứng tự động hiện có.
