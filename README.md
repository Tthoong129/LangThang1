# 🧭 Lang Thang — Nền Tảng Review Du Lịch & Ẩm Thực Việt Nam

Hệ thống đánh giá, khám phá điểm đến du lịch, ẩm thực đặc sản 3 miền và quản lý địa điểm toàn diện với cơ chế phân quyền đa cấp bậc (Admin Hệ Thống & Admin Cấp 1).

---

## 🚀 Cách Chạy Ứng Dụng Với Docker (Nhanh Nhất)

### Yêu cầu:
- Đã cài đặt **Docker & Docker Desktop**.

### Chạy hệ thống:
```bash
docker compose up --build
```
> 💡 Container **Microsoft SQL Server 2022** và **ASP.NET Core Web App** sẽ tự khởi động, tự động tạo CSDL `TravelReviewDB_v2`, 24 bảng dữ liệu và nạp sẵn toàn bộ dữ liệu mẫu (Seed Data) địa điểm, món ăn, tài khoản.

👉 **Truy cập:** [http://localhost:5192](http://localhost:5192)

---

## 🔑 Tài Khoản Mặc Định

| Vai trò | Email | Mật khẩu | Chức năng |
| :--- | :--- | :--- | :--- |
| **Admin Hệ Thống** | `admin@travelreview.vn` | `Admin@123` | Quản trị toàn hệ thống, tài khoản, danh mục, cấu hình |
| **Admin Cấp 1 (Ẩm thực)** | `admin.cap1@travelreview.vn` | `Admin@123` | Phê duyệt địa điểm, đề xuất sửa đổi danh mục ẩm thực |
| **Thành viên thường** | `an.nguyen@gmail.com` | `User@123` | Đánh giá, bình luận, lưu yêu thích, đề xuất địa điểm |

---

## 🛠️ Công Nghệ Sử Dụng
- **Backend:** C# / .NET 10.0 ASP.NET Core Web API, Entity Framework Core
- **Database:** Microsoft SQL Server 2022
- **Frontend:** HTML5, CSS3 Glassmorphism, JavaScript ES6+, Leaflet Maps, Chart.js
- **DevOps:** Docker, Docker Compose Multi-Stage Build
