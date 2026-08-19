# 🚀 Hướng Dẫn Chạy Dự Án Với Docker (1 Bước Duy Nhất)

Dự án đã được đóng gói hoàn chỉnh bằng **Docker & Docker Compose**, bao gồm:
1. **Microsoft SQL Server 2022** (Chạy trên cổng `1433`).
2. **ASP.NET Core Web App** (Chạy trên cổng `5192`).
3. **Tự động khởi tạo CSDL & Nạp sẵn dữ liệu mẫu**: Toàn bộ 24 bảng, dữ liệu địa điểm, món ăn, tài khoản Admin/User sẽ được tự động tạo và seed ngay khi container khởi động.

---

## ⚡ Cách Chạy (Quick Start)

### Cách 1: Chạy toàn bộ (SQL Server + Web App) bằng Docker

1. Mở Terminal / PowerShell tại thư mục gốc của dự án (`d:\MiniMap`).
2. Chạy lệnh:
```bash
docker compose up --build
```
*(Hoặc chạy ngầm trong background: `docker compose up -d --build`)*

3. Mở trình duyệt truy cập:
👉 **[http://localhost:5192](http://localhost:5192)**

---

### Cách 2: Chỉ chạy SQL Server bằng Docker (Dùng dotnet run ở máy)

Nếu bạn muốn code và debug trực tiếp trên máy:
1. Khởi động SQL Server qua Docker:
```bash
docker compose up -d sqlserver
```
2. Chạy ứng dụng trên máy:
```bash
dotnet run
```
3. Truy cập **[http://localhost:5192](http://localhost:5192)**.

---

## 🔑 Thông Tin Kết Nối SQL Server & Tài Khoản Mặc Định

### 1. Kết nối SQL Server (SSMS / Azure Data Studio / DBeaver):
- **Server**: `localhost,1433` (hoặc `127.0.0.1,1433`)
- **Authentication**: `SQL Server Authentication`
- **Username**: `sa`
- **Password**: `YourStrong@Passw0rd`
- **Database**: `TravelReviewDB_v2`

### 2. Tài khoản đăng nhập sẵn trên Website:
| Vai trò | Email | Mật khẩu | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Admin Hệ Thống** | `admin@travelreview.vn` | `Admin@123` | Toàn quyền quản trị website, user, cấu hình |
| **Admin Cấp 1** | `admin.cap1@travelreview.vn` | `Admin@123` | Quản lý & duyệt bài danh mục Ẩm thực |
| **Admin Du Lịch** | `admin.travel@travelreview.vn` | `Admin@123` | Quản lý & duyệt bài danh mục Du lịch |
| **Người Dùng Thường** | `an.nguyen@gmail.com` | `User@123` | Đăng đánh giá, yêu thích, đề xuất địa điểm |

---

## 🛑 Dừng Hệ Thống

Khi muốn tắt các container Docker:
```bash
docker compose down
```
*(Nếu muốn xóa sạch cả dữ liệu để tạo mới lại từ đầu: `docker compose down -v`)*
