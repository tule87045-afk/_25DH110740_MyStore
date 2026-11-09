# BadmintonShop – Website bán hàng cầu lông (ASP.NET MVC 5 + Entity Framework 6)

Dự án làm theo tài liệu *"Xây dựng website bán hàng bằng ASP.NET MVC5 Entity Framework – Phần Customer Site"*.
Chủ đề: cửa hàng cầu lông **Shuttle Pro** (vợt, cầu, giày, quần áo, túi, phụ kiện).

## Yêu cầu
- Visual Studio 2019/2022 (workload *ASP.NET and web development*), .NET Framework 4.8
- SQL Server LocalDB (đi kèm Visual Studio) hoặc SQL Server/Express

## Chạy dự án
1. Giải nén, mở `BadmintonShop.sln` bằng Visual Studio.
2. Chuột phải Solution → **Restore NuGet Packages**.
3. (Tùy chọn) Sửa chuỗi kết nối `BadmintonShopContext` trong `Web.config` nếu không dùng LocalDB.
4. Nhấn **F5**. Lần chạy đầu tiên, Entity Framework (Code First) tự tạo database `BadmintonShop` và nạp dữ liệu mẫu (6 danh mục, 24 sản phẩm, tài khoản demo).

| Vai trò | URL | Tài khoản |
|---|---|---|
| Khách hàng | `/` | `khach` / `123456` (hoặc tự Đăng ký) |
| Admin | `/Admin` | `admin` / `admin123` |

## Đối chiếu với yêu cầu trong slide
| Yêu cầu | Cài đặt |
|---|---|
| Customer Layout | `Views/Shared/_CustomerLayout.cshtml` (header, menu, footer) |
| Menu danh mục (Partial) | `Views/Shared/_CategoriesPV.cshtml` ← `SharedController.CategoryMenu` |
| Menu cá nhân (Partial, `PersonalMenuVM`) | `Views/Shared/_PersonalMenuPV.cshtml` ← `SharedController.PersonalMenu` (Hello, user / Đăng ký / Đăng nhập / giỏ hàng + số lượng) |
| Đăng ký / Đăng nhập / Đăng xuất | `AccountController` (Register, Login, Logout) + `RegisterVM`, `LoginVM`; Forms Authentication, timeout 3600s trong `Web.config`; lưu Session + `SetAuthCookie(username, remember)` |
| Thông tin tài khoản, đổi mật khẩu | `AccountController.ProfileInfo / UpdateProfile / ChangePassword` |
| Trang Home | `HomeController.Index(searchTerm, page)` + `HomeProductVM`; partial `_PVFeatureProduct` (top 10 bán chạy, dạng slider), `_PVNewProduct` (20 sản phẩm, 6 sản phẩm/trang); tìm theo tên/mô tả/danh mục/thương hiệu |
| Danh sách theo danh mục | `HomeController.ProductList` (tìm, sắp xếp, phân trang) |
| Chi tiết sản phẩm | `HomeController.ProductDetail` + `ProductDetailVM`; partial `_PVRelatedProduct` (8 SP cùng loại, slider), `_PVTopProduct` (bán chạy, 3 SP/trang); chọn số lượng + tạm tính |
| Giỏ hàng nâng cao | `CartController` (Index, AddItem, RemoveItem, ClearCart, UpdateQuantity) + `CartItem`, `Cart` (`GroupedItems` nhóm theo danh mục), `CartService` (Session); partial `_PVSimilarProduct` |
| Thanh toán | `OrderController.Checkout` (GET/POST) + `CheckoutVM`; chọn giao hàng, thanh toán, sửa địa chỉ (`enableAddressEdit`, `updateShippingAddress`); kiểm tra tồn kho |
| Xác nhận đơn hàng | `OrderController.OrderSuccess` + `Views/Order/OrderSuccess.cshtml` |
| Lịch sử mua hàng | `OrderController.MyOrder` – tab theo trạng thái, tìm theo mã đơn / tên sản phẩm, Mua lại, Hủy đơn |
| Admin: danh mục, sản phẩm (CRUD, tìm, sắp xếp, phân trang, upload ảnh) | `Areas/Admin` – `CategoryController`, `ProductController` |
| Admin: khách hàng & đơn hàng | `CustomerController` (tìm/sắp xếp/phân trang, xem đơn của khách), `OrderController` (lọc, chi tiết, đổi trạng thái) |
| Admin: tài khoản Admin (CRUD, tìm, phân trang), đăng nhập/xuất | `AdminUserController`, `Account` |
| Admin: trang Home = thống kê | `DashboardController` |

## Ghi chú
- Mô hình dữ liệu dùng **Code First** (`Models/Entities.cs`, `BadmintonShopContext`) thay cho EDMX để dễ chia sẻ; các trường theo slide: `Order.DeliveryMethod`, `Order.PaymentMethod`, `Order.ShippingAddress`, `OrderDetail.TotalPrice` (= Quantity × UnitPrice, tính trong code).
- Mật khẩu được băm SHA-256 + salt (`Helpers/PasswordHelper.cs`).
- Giao diện dùng Bootstrap 3, Font Awesome qua CDN (cần Internet khi chạy). Ảnh sản phẩm là SVG minh họa trong `Content/images/products` – thay bằng ảnh thật qua trang Admin.
- **PayPal (tùy chọn)**: slide dành phần này là optional. Form Checkout đã có lựa chọn PayPal (đơn ở trạng thái "Chờ thanh toán") và `Web.config` có sẵn khóa `PayPalClientId/ClientSecret/PayPalMode`. Để tích hợp thật: đăng ký PayPal Developer, `Install-Package PayPal`, thêm `PaypalController.PaymentWithPaypal` theo slide 49–51 rồi gọi từ `OrderController.Checkout` khi `PaymentMethod == "PayPal"`.
- Phí ship: cấu hình trong `Web.config` (`FreeShipThreshold`, `StandardShippingFee`, `ExpressShippingFee`).
- Lưu ý: mã chưa được biên dịch trong môi trường tạo file (không có .NET Framework); nếu gặp lỗi build nhỏ khi restore/biên dịch, hãy gửi lại thông báo lỗi để chỉnh.
