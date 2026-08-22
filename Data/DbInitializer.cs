using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MiniMap.Models;

namespace MiniMap.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TravelReviewDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            // 1. Users
            var systemAdmin = new User
            {
                FullName = "Admin Hệ Thống",
                Email = "admin@travelreview.vn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Phone = "0901234567",
                AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150",
                Role = "system_admin",
                Status = "active"
            };

            var catAdminFood = new User
            {
                FullName = "Admin Cấp 1",
                Email = "admin.cap1@travelreview.vn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Phone = "0902345678",
                AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150",
                Role = "category_admin",
                Status = "active"
            };

            var catAdminTravel = new User
            {
                FullName = "Admin Du Lịch & Nghỉ Dưỡng",
                Email = "admin.travel@travelreview.vn",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Phone = "0903456789",
                AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150",
                Role = "category_admin",
                Status = "active"
            };

            var user1 = new User
            {
                FullName = "Cathy_Zhou", // Match the tripadvisor screenshot for fun
                Email = "an.nguyen@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Phone = "0912345678",
                AvatarUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150",
                Role = "user",
                Status = "active",
                Bio = "Tôi là một người yêu thích du lịch và khám phá những điều mới mẻ. Châm ngôn: Đi để học hỏi.",
                CoverUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=1200",
                CreatedAt = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var user2 = new User
            {
                FullName = "Trần Thị Mai",
                Email = "mai.tran@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Phone = "0918765432",
                AvatarUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150",
                Role = "user",
                Status = "active"
            };

            context.Users.AddRange(systemAdmin, catAdminFood, catAdminTravel, user1, user2);
            context.SaveChanges();

            // 2. Regions
            var north = new Region { Name = "Miền Bắc", Status = "active" };
            var central = new Region { Name = "Miền Trung", Status = "active" };
            var south = new Region { Name = "Miền Nam", Status = "active" };
            context.Regions.AddRange(north, central, south);
            context.SaveChanges();

            // 3. Provinces
            var hanoi = new Province { Name = "Hà Nội", RegionId = north.Id, Status = "active" };
            var sapa = new Province { Name = "Lào Cai (Sa Pa)", RegionId = north.Id, Status = "active" };
            var quangninh = new Province { Name = "Quảng Ninh (Hạ Long)", RegionId = north.Id, Status = "active" };

            var danang = new Province { Name = "Đà Nẵng", RegionId = central.Id, Status = "active" };
            var hue = new Province { Name = "Thừa Thiên Huế", RegionId = central.Id, Status = "active" };
            var nhatrang = new Province { Name = "Khánh Hòa (Nha Trang)", RegionId = central.Id, Status = "active" };
            var dalat = new Province { Name = "Lâm Đồng (Đà Lạt)", RegionId = central.Id, Status = "active" };

            var hcm = new Province { Name = "TP. Hồ Chí Minh", RegionId = south.Id, Status = "active" };
            var phuquoc = new Province { Name = "Kiên Giang (Phú Quốc)", RegionId = south.Id, Status = "active" };
            var vungtau = new Province { Name = "Bà Rịa - Vũng Tàu", RegionId = south.Id, Status = "active" };

            context.Provinces.AddRange(hanoi, sapa, quangninh, danang, hue, nhatrang, dalat, hcm, phuquoc, vungtau);
            context.SaveChanges();

            // 4. PlaceTypes
            var typeFood = new PlaceType { Name = "Ăn uống", Status = "active" };
            var typeTravel = new PlaceType { Name = "Du lịch", Status = "active" };
            var typeStay = new PlaceType { Name = "Lưu trú", Status = "active" };
            var typePlay = new PlaceType { Name = "Vui chơi", Status = "active" };
            context.PlaceTypes.AddRange(typeFood, typeTravel, typeStay, typePlay);
            context.SaveChanges();

            // 5. Categories
            var catRestaurant = new Category { Name = "Nhà hàng & Quán ăn", PlaceTypeId = typeFood.Id, Status = "active" };
            var catCafe = new Category { Name = "Quán Cà phê & Trà", PlaceTypeId = typeFood.Id, Status = "active" };
            var catStreetFood = new Category { Name = "Ẩm thực đường phố", PlaceTypeId = typeFood.Id, Status = "active" };

            var catBeach = new Category { Name = "Bãi biển & Đảo", PlaceTypeId = typeTravel.Id, Status = "active" };
            var catNature = new Category { Name = "Núi, Thác & Thiên nhiên", PlaceTypeId = typeTravel.Id, Status = "active" };
            var catHistory = new Category { Name = "Di tích & Danh lam thắng cảnh", PlaceTypeId = typeTravel.Id, Status = "active" };

            var catHotel = new Category { Name = "Khách sạn cao cấp", PlaceTypeId = typeStay.Id, Status = "active" };
            var catResort = new Category { Name = "Resort nghỉ dưỡng", PlaceTypeId = typeStay.Id, Status = "active" };
            var catHomestay = new Category { Name = "Homestay & Villa", PlaceTypeId = typeStay.Id, Status = "active" };

            var catThemePark = new Category { Name = "Công viên & Khu vui chơi giải trí", PlaceTypeId = typePlay.Id, Status = "active" };
            var catEntertainment = new Category { Name = "Rạp phim & Trải nghiệm đêm", PlaceTypeId = typePlay.Id, Status = "active" };

            context.Categories.AddRange(
                catRestaurant, catCafe, catStreetFood,
                catBeach, catNature, catHistory,
                catHotel, catResort, catHomestay,
                catThemePark, catEntertainment
            );
            context.SaveChanges();

            // 6. Admin Category Assignments
            context.AdminCategoryAssignments.AddRange(
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catRestaurant.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catCafe.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catStreetFood.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catBeach.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catNature.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catHistory.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catHotel.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catResort.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catHomestay.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catThemePark.Id },
                new AdminCategoryAssignment { UserId = catAdminFood.Id, CategoryId = catEntertainment.Id },
                new AdminCategoryAssignment { UserId = catAdminTravel.Id, CategoryId = catBeach.Id },
                new AdminCategoryAssignment { UserId = catAdminTravel.Id, CategoryId = catNature.Id },
                new AdminCategoryAssignment { UserId = catAdminTravel.Id, CategoryId = catResort.Id },
                new AdminCategoryAssignment { UserId = catAdminTravel.Id, CategoryId = catHomestay.Id }
            );
            context.SaveChanges();

            // 7. Report Reasons
            var r1 = new ReportReason { Content = "Thông tin địa điểm sai lệch hoặc không tồn tại", Status = "active" };
            var r2 = new ReportReason { Content = "Nội dung spam, quảng cáo rác hoặc xúc phạm", Status = "active" };
            var r3 = new ReportReason { Content = "Hình ảnh hoặc video không phù hợp thuần phong mỹ tục", Status = "active" };
            var r4 = new ReportReason { Content = "Đánh giá giả mạo, vu khống hoặc cạnh tranh không lành mạnh", Status = "active" };
            context.ReportReasons.AddRange(r1, r2, r3, r4);

            // 8. System Configs
            context.SystemConfigs.AddRange(
                new SystemConfig { ConfigKey = "MAX_IMAGE_COUNT_PER_REVIEW", ConfigValue = "5", Description = "Số lượng hình ảnh tối đa cho mỗi bài đánh giá", UpdatedBy = systemAdmin.Id },
                new SystemConfig { ConfigKey = "MAX_IMAGE_SIZE_MB", ConfigValue = "10", Description = "Dung lượng ảnh tối đa mỗi file (MB)", UpdatedBy = systemAdmin.Id },
                new SystemConfig { ConfigKey = "ALLOWED_IMAGE_EXTENSIONS", ConfigValue = "jpg,jpeg,png,webp", Description = "Định dạng ảnh được hỗ trợ", UpdatedBy = systemAdmin.Id },
                new SystemConfig { ConfigKey = "ENABLE_USER_PROPOSALS", ConfigValue = "true", Description = "Bật/Tắt tính năng cho phép người dùng đề xuất địa điểm", UpdatedBy = systemAdmin.Id }
            );
            context.SaveChanges();

            // 9. Places
            var p1 = new Place
            {
                Name = "Phở Thìn Lò Đúc",
                Description = "Quán phở bò xào lăn trứ danh Hà Nội với nước dùng béo ngậy đậm đà và hành lá ngập bát từ năm 1979.",
                Address = "13 Lò Đúc, Phạm Đình Hổ, Hai Bà Trưng, Hà Nội",
                Phone = "02439434455",
                Website = "https://phothin.vn",
                MinPrice = 65000,
                MaxPrice = 110000,
                OpeningHours = "06:00 - 21:00",
                Latitude = 21.0182810m,
                Longitude = 105.8569850m,
                ProvinceId = hanoi.Id,
                CategoryId = catRestaurant.Id,
                Status = "active",
                AvgRating = 4.8m,
                ReviewCount = 24
            };

            var p2 = new Place
            {
                Name = "Cà phê Giảng (Cà phê Trứng Cổ Hà Nội)",
                Description = "Nơi khởi nguồn món Cà phê trứng độc nhất vô nhị từ năm 1946 tại phố cổ Hà Nội.",
                Address = "39 Nguyễn Hữu Huân, Hoàn Kiếm, Hà Nội",
                Phone = "0989892298",
                MinPrice = 35000,
                MaxPrice = 60000,
                OpeningHours = "07:00 - 22:30",
                Latitude = 21.0336710m,
                Longitude = 105.8543410m,
                ProvinceId = hanoi.Id,
                CategoryId = catCafe.Id,
                Status = "active",
                AvgRating = 4.9m,
                ReviewCount = 45
            };

            var p3 = new Place
            {
                Name = "Bà Nà Hills & Cầu Vàng",
                Description = "Khu du lịch trên đỉnh núi Chúa với Cầu Vàng nâng bởi bàn tay khổng lồ nổi tiếng toàn cầu và làng Pháp mộng mơ.",
                Address = "Thôn An Sơn, Hòa Ninh, Hòa Vang, Đà Nẵng",
                Phone = "02363791999",
                Website = "https://banahills.sunworld.vn",
                MinPrice = 600000,
                MaxPrice = 1250000,
                OpeningHours = "07:30 - 21:30",
                Latitude = 15.9989670m,
                Longitude = 107.9866120m,
                ProvinceId = danang.Id,
                CategoryId = catThemePark.Id,
                Status = "active",
                AvgRating = 4.9m,
                ReviewCount = 88
            };

            var p4 = new Place
            {
                Name = "InterContinental Danang Sun Peninsula Resort",
                Description = "Khu nghỉ dưỡng 5 sao sang trọng bậc nhất tọa lạc tại bán đảo Sơn Trà nhìn thẳng ra vịnh biển xanh ngọc bích.",
                Address = "Bãi Bắc, Bán đảo Sơn Trà, Đà Nẵng",
                Phone = "02363938888",
                Website = "https://danang.intercontinental.com",
                MinPrice = 8500000,
                MaxPrice = 35000000,
                OpeningHours = "24/7",
                Latitude = 16.1207120m,
                Longitude = 108.3101560m,
                ProvinceId = danang.Id,
                CategoryId = catResort.Id,
                Status = "active",
                AvgRating = 5.0m,
                ReviewCount = 65
            };

            var p5 = new Place
            {
                Name = "Quán Bụi - Hương Vị Quê Nhà Sài Gòn",
                Description = "Không gian ẩm thực Việt truyền thống phong cách Đông Dương ấm cúng giữa lòng trung tâm Quận 1.",
                Address = "19 Ngô Văn Năm, Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                Phone = "02838291515",
                MinPrice = 150000,
                MaxPrice = 450000,
                OpeningHours = "07:00 - 23:00",
                Latitude = 10.7801830m,
                Longitude = 106.7052980m,
                ProvinceId = hcm.Id,
                CategoryId = catRestaurant.Id,
                Status = "active",
                AvgRating = 4.7m,
                ReviewCount = 19
            };

            var p6 = new Place
            {
                Name = "Bãi Sao Phú Quốc",
                Description = "Bãi biển cát trắng mịn như kem và làn nước trong vắt êm đềm top đầu Việt Nam.",
                Address = "Ấp 4, An Thới, TP. Phú Quốc, Kiên Giang",
                OpeningHours = "06:00 - 18:30",
                Latitude = 10.0543660m,
                Longitude = 104.0326550m,
                ProvinceId = phuquoc.Id,
                CategoryId = catBeach.Id,
                Status = "active",
                AvgRating = 4.8m,
                ReviewCount = 52
            };

            var p7 = new Place
            {
                Name = "Quán Cà Phê Mê Linh Coffee Garden",
                Description = "Quán cà phê ngắm đồi chè và cánh đồng hoa bạt ngàn tại Đà Lạt với cà phê chồn nguyên chất.",
                Address = "Tổ 20, Thôn 4, Tà Nung, TP. Đà Lạt, Lâm Đồng",
                Phone = "0919619888",
                MinPrice = 40000,
                MaxPrice = 120000,
                OpeningHours = "07:00 - 18:00",
                Latitude = 11.9056200m,
                Longitude = 108.3512300m,
                ProvinceId = dalat.Id,
                CategoryId = catCafe.Id,
                Status = "active",
                AvgRating = 4.6m,
                ReviewCount = 28
            };

            context.Places.AddRange(p1, p2, p3, p4, p5, p6, p7);
            context.SaveChanges();

            // 10. Place Media
            context.PlaceMedia.AddRange(
                new PlaceMedia { PlaceId = p1.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=800" },
                new PlaceMedia { PlaceId = p1.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=800" },
                new PlaceMedia { PlaceId = p2.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=800" },
                new PlaceMedia { PlaceId = p3.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800" },
                new PlaceMedia { PlaceId = p3.Id, MediaType = "video", Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ" },
                new PlaceMedia { PlaceId = p4.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800" },
                new PlaceMedia { PlaceId = p5.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800" },
                new PlaceMedia { PlaceId = p6.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800" },
                new PlaceMedia { PlaceId = p7.Id, MediaType = "image", Url = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=800" }
            );
            context.SaveChanges();

            // 11. Foods
            var f1 = new Food
            {
                Name = "Phở Bò Hà Nội",
                Description = "Món ăn quốc hồn quốc túy với bánh phở dẻo mềm, thịt bò tươi ngọt và nước dùng ninh xương thơm thảo mộc quế hồi.",
                ImageUrl = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=600"
            };

            var f2 = new Food
            {
                Name = "Cà Phê Trứng",
                Description = "Thức uống độc đáo sáng tạo từ lòng đỏ trứng gà đánh bông mịn cùng cà phê Robusta sánh thơm.",
                ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=600"
            };

            var f3 = new Food
            {
                Name = "Mì Quảng Đà Nẵng",
                Description = "Mì sợi dai vàng ươm dùng kèm tôm, thịt, trứng cút, bánh tráng nướng giòn rụm và rau sống tươi xanh.",
                ImageUrl = "https://images.unsplash.com/photo-1617093727343-374698b1b08d?w=600"
            };

            var f4 = new Food
            {
                Name = "Bánh Mì Sài Gòn",
                Description = "Bánh mì vỏ giòn xốp nhân pate gan béo ngậy, thịt nguội, chả lụa, dưa chua và sốt ớt cay nồng trứ danh thế giới.",
                ImageUrl = "https://images.unsplash.com/photo-1627308595229-7830a5c91f9f?w=600"
            };

            context.Foods.AddRange(f1, f2, f3, f4);
            context.SaveChanges();

            // Food Provinces
            context.FoodProvinces.AddRange(
                new FoodProvince { FoodId = f1.Id, ProvinceId = hanoi.Id },
                new FoodProvince { FoodId = f2.Id, ProvinceId = hanoi.Id },
                new FoodProvince { FoodId = f3.Id, ProvinceId = danang.Id },
                new FoodProvince { FoodId = f4.Id, ProvinceId = hcm.Id }
            );

            // Food Places
            context.FoodPlaces.AddRange(
                new FoodPlace { FoodId = f1.Id, PlaceId = p1.Id },
                new FoodPlace { FoodId = f2.Id, PlaceId = p2.Id },
                new FoodPlace { FoodId = f4.Id, PlaceId = p5.Id }
            );
            context.SaveChanges();

            // 12. Reviews
            var rev1 = new Review
            {
                PlaceId = p1.Id,
                UserId = user1.Id,
                Rating = 5,
                Content = "\"Our Tour guide Peter spiderman was very great. The tunnel was amazing. It helped me know more about the war.\"\n\nPhở bò xào lăn ở đây rất xuất sắc, nước dùng béo ngậy thơm nức mũi! Quán đông nhưng phục vụ rất nhanh nhẹn.",
                Status = "visible",
                ExperienceDate = DateTime.UtcNow.AddDays(-15)
            };

            var rev2 = new Review
            {
                PlaceId = p2.Id,
                UserId = user2.Id,
                Rating = 5,
                Content = "Cà phê trứng ngậy béo, thơm lừng và hoàn toàn không bị tanh. Không gian đậm chất phố cổ xưa rất chill!",
                Status = "visible",
                ExperienceDate = DateTime.UtcNow.AddDays(-30)
            };

            context.Reviews.AddRange(rev1, rev2);
            context.SaveChanges();

            // Review Media
            context.ReviewMedia.AddRange(
                new ReviewMedia { ReviewId = rev1.Id, ImageUrl = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=1200" },
                new ReviewMedia { ReviewId = rev2.Id, ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=1200" }
            );
            context.SaveChanges();

            // 13. Comments
            context.Comments.Add(new Comment
            {
                ReviewId = rev1.Id,
                UserId = user2.Id,
                Content = "Đồng ý với bạn, nhớ gọi thêm quẩy giòn ăn kèm thì chuẩn bài luôn nha!",
                Status = "visible"
            });

            // 14. Favorites
            context.Favorites.Add(new Favorite
            {
                UserId = user1.Id,
                PlaceId = p2.Id
            });

            // 15. VisitLogs
            context.VisitLogs.Add(new VisitLog
            {
                UserId = user1.Id,
                PlaceId = p1.Id,
                VisitedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                Privacy = "public"
            });

            // 16. AccessHistory
            context.AccessHistories.Add(new AccessHistory
            {
                UserId = user1.Id,
                PlaceId = p1.Id,
                ViewedAt = DateTime.UtcNow.AddHours(-1)
            });

            // 17. Notifications
            context.Notifications.Add(new Notification
            {
                UserId = user1.Id,
                Content = "Chào mừng bạn đến với hệ thống TravelReview! Hãy cùng khám phá và đánh giá các địa điểm tuyệt vời.",
                Type = "place_approved",
                IsRead = false
            });

            context.SaveChanges();
        }
    }
}
