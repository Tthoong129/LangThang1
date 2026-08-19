using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniMap.Data;
using MiniMap.Models;

namespace MiniMap.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> AddOrUpdateReviewAsync(long userId, AddReviewDto dto);
        Task<bool> DeleteReviewAsync(long userId, long reviewId, bool isAdmin = false);
        Task<CommentDto> AddCommentAsync(long userId, AddCommentDto dto);
        Task<bool> DeleteCommentAsync(long userId, long commentId, bool isAdmin = false);
        Task<bool> ReportTargetAsync(long userId, CreateReportDto dto);
        Task<bool> SubmitAppealAsync(long userId, CreateAppealDto dto);
    }

    public class AddReviewDto
    {
        public long PlaceId { get; set; }
        public byte Rating { get; set; } // 1-5
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public class AddCommentDto
    {
        public long ReviewId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class CreateReportDto
    {
        public string TargetType { get; set; } = string.Empty; // 'place','review','comment'
        public long TargetId { get; set; }
        public int ReasonId { get; set; }
        public string? Description { get; set; }
    }

    public class CreateAppealDto
    {
        public string TargetType { get; set; } = string.Empty; // 'place','review','comment','place_edit_proposal'
        public long TargetId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ReviewService : IReviewService
    {
        private readonly TravelReviewDbContext _db;
        private readonly IPlaceService _placeService;

        public ReviewService(TravelReviewDbContext db, IPlaceService placeService)
        {
            _db = db;
            _placeService = placeService;
        }

        public async Task<ReviewDto> AddOrUpdateReviewAsync(long userId, AddReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                throw new ArgumentException("Điểm đánh giá phải từ 1 đến 5 sao.");
            }

            var existing = await _db.Reviews
                .Include(r => r.Media)
                .FirstOrDefaultAsync(r => r.PlaceId == dto.PlaceId && r.UserId == userId);

            Review review;
            if (existing != null)
            {
                existing.Rating = dto.Rating;
                existing.Content = dto.Content;
                existing.VideoUrl = dto.VideoUrl;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.Status = "visible";

                // Replace media
                _db.ReviewMedia.RemoveRange(existing.Media);
                review = existing;
            }
            else
            {
                review = new Review
                {
                    PlaceId = dto.PlaceId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Content = dto.Content,
                    VideoUrl = dto.VideoUrl,
                    Status = "visible",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Reviews.Add(review);
            }

            await _db.SaveChangesAsync();

            if (dto.ImageUrls != null)
            {
                foreach (var img in dto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.ReviewMedia.Add(new ReviewMedia
                    {
                        ReviewId = review.Id,
                        ImageUrl = img
                    });
                }
                await _db.SaveChangesAsync();
            }

            // Recalculate place rating
            await _placeService.RecalculatePlaceRatingAsync(dto.PlaceId);

            var user = await _db.Users.FindAsync(userId);

            return new ReviewDto
            {
                Id = review.Id,
                UserId = userId,
                UserName = user?.FullName ?? "Người dùng",
                UserAvatar = user?.AvatarUrl,
                Rating = review.Rating,
                Content = review.Content,
                VideoUrl = review.VideoUrl,
                CreatedAt = review.CreatedAt,
                Images = dto.ImageUrls ?? new()
            };
        }

        public async Task<bool> DeleteReviewAsync(long userId, long reviewId, bool isAdmin = false)
        {
            var review = await _db.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (!isAdmin && review.UserId != userId)
            {
                return false;
            }

            long placeId = review.PlaceId;
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();

            // Recalculate place rating!
            await _placeService.RecalculatePlaceRatingAsync(placeId);
            return true;
        }

        public async Task<CommentDto> AddCommentAsync(long userId, AddCommentDto dto)
        {
            var review = await _db.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == dto.ReviewId);
            if (review == null)
            {
                throw new InvalidOperationException("Bài đánh giá không tồn tại.");
            }

            var comment = new Comment
            {
                ReviewId = dto.ReviewId,
                UserId = userId,
                Content = dto.Content.Trim(),
                Status = "visible",
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            var author = await _db.Users.FindAsync(userId);

            // Notify review owner if someone else commented
            if (review.UserId != userId)
            {
                _db.Notifications.Add(new Notification
                {
                    UserId = review.UserId,
                    Content = $"{author?.FullName ?? "Một người dùng"} đã bình luận về bài đánh giá của bạn.",
                    Type = "comment_created",
                    TargetType = "review",
                    TargetId = review.Id,
                    IsRead = false
                });
                await _db.SaveChangesAsync();
            }

            return new CommentDto
            {
                Id = comment.Id,
                UserId = userId,
                UserName = author?.FullName ?? "Người dùng",
                UserAvatar = author?.AvatarUrl,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<bool> DeleteCommentAsync(long userId, long commentId, bool isAdmin = false)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null) return false;

            if (!isAdmin && comment.UserId != userId)
            {
                return false;
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReportTargetAsync(long userId, CreateReportDto dto)
        {
            var existing = await _db.Reports.AnyAsync(r =>
                r.ReporterId == userId &&
                r.TargetType == dto.TargetType &&
                r.TargetId == dto.TargetId);

            if (existing)
            {
                throw new InvalidOperationException("Bạn đã gửi báo cáo cho nội dung này rồi.");
            }

            var report = new Report
            {
                ReporterId = userId,
                TargetType = dto.TargetType,
                TargetId = dto.TargetId,
                ReasonId = dto.ReasonId,
                Description = dto.Description,
                Status = "pending",
                SubmittedAt = DateTime.UtcNow
            };

            _db.Reports.Add(report);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitAppealAsync(long userId, CreateAppealDto dto)
        {
            var appeal = new Appeal
            {
                UserId = userId,
                TargetType = dto.TargetType,
                TargetId = dto.TargetId,
                Reason = dto.Reason,
                Status = "pending",
                SubmittedAt = DateTime.UtcNow
            };

            _db.Appeals.Add(appeal);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
