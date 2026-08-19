using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniMap.Data;
using MiniMap.Models;

namespace MiniMap.Services
{
    public interface IUserService
    {
        Task<UserDto?> AuthenticateAsync(string email, string password);
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<UserDto?> GetProfileAsync(long userId);
        Task<PublicProfileDto?> GetPublicProfileAsync(long userId);
        Task<List<UserReviewDto>> GetUserReviewsAsync(long userId);
        Task<bool> UpdateProfileAsync(long userId, UpdateProfileDto dto);
        Task<bool> ChangePasswordAsync(long userId, string oldPassword, string newPassword);

        // Favorites
        Task<bool> ToggleFavoriteAsync(long userId, long placeId);
        Task<List<PlaceDto>> GetFavoritesAsync(long userId);

        // Visit Logs
        Task<VisitLogDto> AddVisitLogAsync(long userId, AddVisitLogDto dto);
        Task<bool> RemoveVisitLogAsync(long userId, long logId);
        Task<List<VisitLogDto>> GetUserVisitLogsAsync(long userId, bool isOwner = true);

        // Access History
        Task<List<AccessHistoryDto>> GetAccessHistoryAsync(long userId);
        Task<bool> RemoveAccessHistoryItemAsync(long userId, long historyId);
        Task<bool> ClearAccessHistoryAsync(long userId);

        // Notifications
        Task<List<NotificationDto>> GetNotificationsAsync(long userId);
        Task<bool> MarkNotificationReadAsync(long userId, long notificationId);
    }

    public class UserDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = "user";
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
        public List<int> AssignedCategoryIds { get; set; } = new();
    }

    public class RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class PublicProfileDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? CoverUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime JoinedAt { get; set; }
        public int ReviewCount { get; set; }
        public int PhotoCount { get; set; }
        public List<string> Achievements { get; set; } = new();
    }

    public class UserReviewDto
    {
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public decimal PlaceAvgRating { get; set; }
        public int PlaceReviewCount { get; set; }
        public byte Rating { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExperienceDate { get; set; }
        public List<string> Images { get; set; } = new();
    }

    public class AddVisitLogDto
    {
        public long PlaceId { get; set; }
        public DateOnly VisitedDate { get; set; }
        public string Privacy { get; set; } = "public"; // 'public','private'
    }

    public class VisitLogDto
    {
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public string? PlaceThumbnail { get; set; }
        public string? PlaceAddress { get; set; }
        public DateOnly VisitedDate { get; set; }
        public string Privacy { get; set; } = "public";
        public DateTime CreatedAt { get; set; }
    }

    public class AccessHistoryDto
    {
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public string? PlaceThumbnail { get; set; }
        public string? PlaceAddress { get; set; }
        public decimal AvgRating { get; set; }
        public DateTime ViewedAt { get; set; }
    }

    public class NotificationDto
    {
        public long Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? TargetType { get; set; }
        public long? TargetId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly TravelReviewDbContext _db;

        public UserService(TravelReviewDbContext db)
        {
            _db = db;
        }

        public async Task<UserDto?> AuthenticateAsync(string email, string password)
        {
            var user = await _db.Users
                .Include(u => u.CategoryAssignments)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());

            if (user == null || user.Status == "locked") return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid) return null;

            return MapToDto(user);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower());
            if (existing)
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ?
                    "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150" : dto.AvatarUrl,
                Role = "user",
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Send welcome notification
            _db.Notifications.Add(new Notification
            {
                UserId = user.Id,
                Content = "Chào mừng bạn đến với TravelReview! Hãy cùng khám phá và chia sẻ các địa điểm tuyệt vời.",
                Type = "place_approved",
                IsRead = false
            });
            await _db.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserDto?> GetProfileAsync(long userId)
        {
            var user = await _db.Users
                .Include(u => u.CategoryAssignments)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<PublicProfileDto?> GetPublicProfileAsync(long userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var reviewCount = await _db.Reviews.CountAsync(r => r.UserId == userId && r.Status == "visible");
            
            // To count photos uploaded by user via reviews (roughly)
            var photoCount = await _db.ReviewMedia
                .Where(m => m.Review!.UserId == userId)
                .CountAsync();

            var achievements = new List<string>();
            if (reviewCount >= 1) achievements.Add("Review Beginner");
            if (reviewCount >= 10) achievements.Add("Review Expert");
            if (photoCount >= 1) achievements.Add("Photos Rookie");
            if (photoCount >= 5) achievements.Add("Photos Pro");

            return new PublicProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                CoverUrl = user.CoverUrl,
                Bio = user.Bio,
                JoinedAt = user.CreatedAt,
                ReviewCount = reviewCount,
                PhotoCount = photoCount,
                Achievements = achievements
            };
        }

        public async Task<List<UserReviewDto>> GetUserReviewsAsync(long userId)
        {
            var reviews = await _db.Reviews
                .Include(r => r.Place)
                .Include(r => r.Media)
                .Where(r => r.UserId == userId && r.Status == "visible")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new UserReviewDto
            {
                Id = r.Id,
                PlaceId = r.PlaceId,
                PlaceName = r.Place?.Name ?? "",
                PlaceAvgRating = r.Place?.AvgRating ?? 0,
                PlaceReviewCount = r.Place?.ReviewCount ?? 0,
                Rating = r.Rating,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                ExperienceDate = r.ExperienceDate,
                Images = r.Media.Select(m => m.ImageUrl).ToList()
            }).ToList();
        }

        public async Task<bool> UpdateProfileAsync(long userId, UpdateProfileDto dto)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            user.FullName = dto.FullName.Trim();
            user.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
            {
                user.AvatarUrl = dto.AvatarUrl;
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(long userId, string oldPassword, string newPassword)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleFavoriteAsync(long userId, long placeId)
        {
            var fav = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.PlaceId == placeId);
            if (fav != null)
            {
                _db.Favorites.Remove(fav);
                await _db.SaveChangesAsync();
                return false; // Removed
            }
            else
            {
                _db.Favorites.Add(new Favorite { UserId = userId, PlaceId = placeId, CreatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
                return true; // Added
            }
        }

        public async Task<List<PlaceDto>> GetFavoritesAsync(long userId)
        {
            var favs = await _db.Favorites
                .Include(f => f.Place).ThenInclude(p => p!.Province)
                .Include(f => f.Place).ThenInclude(p => p!.Category).ThenInclude(c => c!.PlaceType)
                .Include(f => f.Place).ThenInclude(p => p!.Media)
                .Where(f => f.UserId == userId && f.Place != null && f.Place.Status == "approved")
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return favs.Select(f => new PlaceDto
            {
                Id = f.Place!.Id,
                Name = f.Place.Name,
                Description = f.Place.Description,
                Address = f.Place.Address,
                AvgRating = f.Place.AvgRating,
                ReviewCount = f.Place.ReviewCount,
                ProvinceName = f.Place.Province?.Name ?? "",
                CategoryName = f.Place.Category?.Name ?? "",
                PlaceTypeName = f.Place.Category?.PlaceType?.Name ?? "",
                ThumbnailUrl = f.Place.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ?? "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500",
                Latitude = f.Place.Latitude,
                Longitude = f.Place.Longitude
            }).ToList();
        }

        public async Task<VisitLogDto> AddVisitLogAsync(long userId, AddVisitLogDto dto)
        {
            var log = new VisitLog
            {
                UserId = userId,
                PlaceId = dto.PlaceId,
                VisitedDate = dto.VisitedDate,
                Privacy = dto.Privacy == "private" ? "private" : "public",
                CreatedAt = DateTime.UtcNow
            };

            _db.VisitLogs.Add(log);
            await _db.SaveChangesAsync();

            var place = await _db.Places.Include(p => p.Media).FirstOrDefaultAsync(p => p.Id == dto.PlaceId);

            return new VisitLogDto
            {
                Id = log.Id,
                PlaceId = log.PlaceId,
                PlaceName = place?.Name ?? "",
                PlaceAddress = place?.Address ?? "",
                PlaceThumbnail = place?.Media.FirstOrDefault(m => m.MediaType == "image")?.Url,
                VisitedDate = log.VisitedDate,
                Privacy = log.Privacy,
                CreatedAt = log.CreatedAt
            };
        }

        public async Task<bool> RemoveVisitLogAsync(long userId, long logId)
        {
            var log = await _db.VisitLogs.FirstOrDefaultAsync(v => v.Id == logId && v.UserId == userId);
            if (log == null) return false;

            _db.VisitLogs.Remove(log);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<VisitLogDto>> GetUserVisitLogsAsync(long userId, bool isOwner = true)
        {
            var query = _db.VisitLogs
                .Include(v => v.Place).ThenInclude(p => p!.Media)
                .Where(v => v.UserId == userId);

            if (!isOwner)
            {
                query = query.Where(v => v.Privacy == "public");
            }

            var logs = await query.OrderByDescending(v => v.VisitedDate).ToListAsync();

            return logs.Select(v => new VisitLogDto
            {
                Id = v.Id,
                PlaceId = v.PlaceId,
                PlaceName = v.Place?.Name ?? "",
                PlaceAddress = v.Place?.Address ?? "",
                PlaceThumbnail = v.Place?.Media.FirstOrDefault(m => m.MediaType == "image")?.Url,
                VisitedDate = v.VisitedDate,
                Privacy = v.Privacy,
                CreatedAt = v.CreatedAt
            }).ToList();
        }

        public async Task<List<AccessHistoryDto>> GetAccessHistoryAsync(long userId)
        {
            var histories = await _db.AccessHistories
                .Include(h => h.Place).ThenInclude(p => p!.Media)
                .Where(h => h.UserId == userId && h.Place != null && h.Place.Status == "approved")
                .OrderByDescending(h => h.ViewedAt)
                .Take(30)
                .ToListAsync();

            return histories.Select(h => new AccessHistoryDto
            {
                Id = h.Id,
                PlaceId = h.PlaceId,
                PlaceName = h.Place!.Name,
                PlaceAddress = h.Place.Address,
                AvgRating = h.Place.AvgRating,
                PlaceThumbnail = h.Place.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ?? "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500",
                ViewedAt = h.ViewedAt
            }).ToList();
        }

        public async Task<bool> RemoveAccessHistoryItemAsync(long userId, long historyId)
        {
            var h = await _db.AccessHistories.FirstOrDefaultAsync(x => x.Id == historyId && x.UserId == userId);
            if (h == null) return false;

            _db.AccessHistories.Remove(h);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearAccessHistoryAsync(long userId)
        {
            var items = await _db.AccessHistories.Where(h => h.UserId == userId).ToListAsync();
            _db.AccessHistories.RemoveRange(items);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(long userId)
        {
            var list = await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(40)
                .ToListAsync();

            return list.Select(n => new NotificationDto
            {
                Id = n.Id,
                Content = n.Content,
                Type = n.Type,
                TargetType = n.TargetType,
                TargetId = n.TargetId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task<bool> MarkNotificationReadAsync(long userId, long notificationId)
        {
            var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);
            if (n == null) return false;

            n.IsRead = true;
            await _db.SaveChangesAsync();
            return true;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                AssignedCategoryIds = user.CategoryAssignments?.Select(c => c.CategoryId).ToList() ?? new()
            };
        }
    }
}
