using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniMap.Data;
using MiniMap.Models;

namespace MiniMap.Services
{
    public interface IAdminService
    {
        // Category Admin Operations
        Task<List<PlaceDto>> GetPendingPlacesForAdminAsync(long adminUserId, bool isSystemAdmin = false);
        Task<bool> ApprovePlaceAsync(long placeId, long adminUserId);
        Task<bool> RejectPlaceAsync(long placeId, long adminUserId, string reason);

        Task<List<PlaceEditProposalDto>> GetPendingEditProposalsAsync(long adminUserId, bool isSystemAdmin = false);
        Task<bool> ApproveEditProposalAsync(long proposalId, long adminUserId);
        Task<bool> RejectEditProposalAsync(long proposalId, long adminUserId, string reason);

        Task<List<ReportDto>> GetReportsForAdminAsync(long adminUserId, bool isSystemAdmin = false);
        Task<bool> ResolveReportAsync(long reportId, long adminUserId, bool confirmViolation, string? note = null);

        Task<List<AppealDto>> GetAppealsAsync(long adminUserId, bool isSystemAdmin = false);
        Task<bool> HandleAppealAsync(long appealId, long adminUserId, bool isSystemAdmin, string result, bool escalate = false);

        // System Admin Operations
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> ToggleUserStatusAsync(long targetUserId, long adminUserId);
        Task<bool> UpdateUserRoleAsync(long targetUserId, string newRole, long adminUserId);
        Task<bool> AssignCategoriesToAdminAsync(long categoryAdminId, List<int> categoryIds, long adminUserId);

        Task<List<SystemConfig>> GetSystemConfigsAsync();
        Task<bool> UpdateSystemConfigAsync(string key, string value, long adminUserId);

        Task<List<AuditLogDto>> GetAuditLogsAsync(int limit = 50);
        Task AddAuditLogAsync(long userId, string action, string? targetType, long? targetId, string? desc);

        Task<DashboardStatsDto> GetAdminDashboardStatsAsync(long adminUserId, bool isSystemAdmin = false);
    }

    public class DashboardStatsDto
    {
        public int TotalPlaces { get; set; }
        public int PendingPlaces { get; set; }
        public int TotalReviews { get; set; }
        public int UserInteractions { get; set; }
        public int PendingReports { get; set; }
    }

    public class PlaceEditProposalDto
    {
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public long ProposedBy { get; set; }
        public string ProposerName { get; set; } = string.Empty;
        public ProposeEditDto? ProposedData { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime SubmittedAt { get; set; }
    }

    public class ReportDto
    {
        public long Id { get; set; }
        public long ReporterId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
        public string TargetTitle { get; set; } = string.Empty;
        public string ReasonContent { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime SubmittedAt { get; set; }
    }

    public class AppealDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public DateTime SubmittedAt { get; set; }
        public string? CategoryAdminResult { get; set; }
        public string? FinalResult { get; set; }
    }

    public class AuditLogDto
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? TargetType { get; set; }
        public long? TargetId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminService : IAdminService
    {
        private readonly TravelReviewDbContext _db;
        private readonly IPlaceService _placeService;

        public AdminService(TravelReviewDbContext db, IPlaceService placeService)
        {
            _db = db;
            _placeService = placeService;
        }

        private async Task<List<int>> GetAdminCategoryIds(long adminUserId, bool isSystemAdmin)
        {
            if (isSystemAdmin) return new List<int>();
            var assigned = await _db.AdminCategoryAssignments
                .Where(a => a.UserId == adminUserId)
                .Select(a => a.CategoryId)
                .ToListAsync();
            
            if (!assigned.Any())
            {
                return await _db.Categories.Select(c => c.Id).ToListAsync();
            }
            return assigned;
        }

        public async Task<List<PlaceDto>> GetPendingPlacesForAdminAsync(long adminUserId, bool isSystemAdmin = false)
        {
            var query = _db.Places
                .Include(p => p.Province)
                .Include(p => p.Category).ThenInclude(c => c!.PlaceType)
                .Include(p => p.Media)
                .Where(p => p.Status == "pending")
                .AsQueryable();

            if (!isSystemAdmin)
            {
                var catIds = await GetAdminCategoryIds(adminUserId, false);
                query = query.Where(p => catIds.Contains(p.CategoryId));
            }

            var list = await query.OrderByDescending(p => p.ProposedAt).ToListAsync();

            return list.Select(p => new PlaceDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Address = p.Address,
                MinPrice = p.MinPrice,
                MaxPrice = p.MaxPrice,
                OpeningHours = p.OpeningHours,
                ProvinceName = p.Province?.Name ?? "",
                CategoryName = p.Category?.Name ?? "",
                PlaceTypeName = p.Category?.PlaceType?.Name ?? "",
                Status = p.Status,
                ThumbnailUrl = p.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ?? "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500"
            }).ToList();
        }

        public async Task<bool> ApprovePlaceAsync(long placeId, long adminUserId)
        {
            var place = await _db.Places.FindAsync(placeId);
            if (place == null) return false;

            place.Status = "approved";
            place.ApprovedBy = adminUserId;
            place.ApprovedAt = DateTime.UtcNow;

            if (place.ProposedBy.HasValue)
            {
                _db.Notifications.Add(new Notification
                {
                    UserId = place.ProposedBy.Value,
                    Content = $"Địa điểm '{place.Name}' bạn đề xuất đã được phê duyệt và hiển thị công khai!",
                    Type = "place_approved",
                    TargetType = "place",
                    TargetId = place.Id
                });
            }

            await AddAuditLogAsync(adminUserId, "APPROVE_PLACE", "place", placeId, $"Duyệt địa điểm: {place.Name}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPlaceAsync(long placeId, long adminUserId, string reason)
        {
            var place = await _db.Places.FindAsync(placeId);
            if (place == null) return false;

            place.Status = "rejected";
            place.RejectReason = reason;
            place.ApprovedBy = adminUserId;
            place.ApprovedAt = DateTime.UtcNow;

            if (place.ProposedBy.HasValue)
            {
                _db.Notifications.Add(new Notification
                {
                    UserId = place.ProposedBy.Value,
                    Content = $"Địa điểm '{place.Name}' bạn đề xuất đã bị từ chối. Lý do: {reason}",
                    Type = "place_rejected",
                    TargetType = "place",
                    TargetId = place.Id
                });
            }

            await AddAuditLogAsync(adminUserId, "REJECT_PLACE", "place", placeId, $"Từ chối địa điểm: {place.Name}. Lý do: {reason}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PlaceEditProposalDto>> GetPendingEditProposalsAsync(long adminUserId, bool isSystemAdmin = false)
        {
            var query = _db.PlaceEditProposals
                .Include(p => p.Place)
                .Include(p => p.Proposer)
                .Where(p => p.Status == "pending")
                .AsQueryable();

            if (!isSystemAdmin)
            {
                var catIds = await GetAdminCategoryIds(adminUserId, false);
                query = query.Where(p => p.Place != null && catIds.Contains(p.Place.CategoryId));
            }

            var list = await query.OrderByDescending(p => p.SubmittedAt).ToListAsync();

            return list.Select(p =>
            {
                ProposeEditDto? data = null;
                try { data = JsonSerializer.Deserialize<ProposeEditDto>(p.ProposedData); } catch { }

                return new PlaceEditProposalDto
                {
                    Id = p.Id,
                    PlaceId = p.PlaceId,
                    PlaceName = p.Place?.Name ?? "",
                    ProposedBy = p.ProposedBy,
                    ProposerName = p.Proposer?.FullName ?? "",
                    ProposedData = data,
                    Status = p.Status,
                    SubmittedAt = p.SubmittedAt
                };
            }).ToList();
        }

        public async Task<bool> ApproveEditProposalAsync(long proposalId, long adminUserId)
        {
            var proposal = await _db.PlaceEditProposals.Include(p => p.Place).FirstOrDefaultAsync(p => p.Id == proposalId);
            if (proposal == null || proposal.Place == null) return false;

            try
            {
                var data = JsonSerializer.Deserialize<ProposeEditDto>(proposal.ProposedData);
                if (data != null)
                {
                    proposal.Place.Name = data.Name;
                    proposal.Place.Description = data.Description;
                    proposal.Place.Address = data.Address;
                    proposal.Place.Phone = data.Phone;
                    proposal.Place.Website = data.Website;
                    proposal.Place.MinPrice = data.MinPrice;
                    proposal.Place.MaxPrice = data.MaxPrice;
                    proposal.Place.OpeningHours = data.OpeningHours;
                    if (data.Latitude.HasValue) proposal.Place.Latitude = data.Latitude;
                    if (data.Longitude.HasValue) proposal.Place.Longitude = data.Longitude;
                    if (data.ProvinceId > 0) proposal.Place.ProvinceId = data.ProvinceId;
                    if (data.CategoryId > 0) proposal.Place.CategoryId = data.CategoryId;
                    proposal.Place.UpdatedAt = DateTime.UtcNow;
                }
            }
            catch { }

            proposal.Status = "approved";
            proposal.ReviewedBy = adminUserId;
            proposal.ReviewedAt = DateTime.UtcNow;

            _db.Notifications.Add(new Notification
            {
                UserId = proposal.ProposedBy,
                Content = $"Đề xuất chỉnh sửa cho địa điểm '{proposal.Place.Name}' đã được chấp nhận và cập nhật vào hệ thống!",
                Type = "place_edit_handled",
                TargetType = "place",
                TargetId = proposal.PlaceId
            });

            await AddAuditLogAsync(adminUserId, "APPROVE_EDIT_PROPOSAL", "place_edit_proposal", proposalId, $"Duyệt chỉnh sửa cho: {proposal.Place.Name}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectEditProposalAsync(long proposalId, long adminUserId, string reason)
        {
            var proposal = await _db.PlaceEditProposals.Include(p => p.Place).FirstOrDefaultAsync(p => p.Id == proposalId);
            if (proposal == null) return false;

            proposal.Status = "rejected";
            proposal.RejectReason = reason;
            proposal.ReviewedBy = adminUserId;
            proposal.ReviewedAt = DateTime.UtcNow;

            _db.Notifications.Add(new Notification
            {
                UserId = proposal.ProposedBy,
                Content = $"Đề xuất chỉnh sửa cho địa điểm '{proposal.Place?.Name ?? ""}' bị từ chối. Lý do: {reason}",
                Type = "place_edit_handled",
                TargetType = "place",
                TargetId = proposal.PlaceId
            });

            await AddAuditLogAsync(adminUserId, "REJECT_EDIT_PROPOSAL", "place_edit_proposal", proposalId, $"Từ chối chỉnh sửa cho ID {proposal.PlaceId}. Lý do: {reason}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReportDto>> GetReportsForAdminAsync(long adminUserId, bool isSystemAdmin = false)
        {
            var query = _db.Reports
                .Include(r => r.Reporter)
                .Include(r => r.Reason)
                .Where(r => r.Status == "pending")
                .AsQueryable();

            if (!isSystemAdmin)
            {
                var catIds = await GetAdminCategoryIds(adminUserId, false);
                query = query.Where(r => 
                    (r.TargetType == "place" && _db.Places.Any(p => p.Id == r.TargetId && catIds.Contains(p.CategoryId))) ||
                    (r.TargetType == "review" && _db.Reviews.Any(rev => rev.Id == r.TargetId && _db.Places.Any(p => p.Id == rev.PlaceId && catIds.Contains(p.CategoryId)))) ||
                    (r.TargetType == "comment" && _db.Comments.Any(c => c.Id == r.TargetId && _db.Reviews.Any(rev => rev.Id == c.ReviewId && _db.Places.Any(p => p.Id == rev.PlaceId && catIds.Contains(p.CategoryId)))))
                );
            }

            var list = await query.OrderByDescending(r => r.SubmittedAt).ToListAsync();

            var result = new List<ReportDto>();
            foreach (var r in list)
            {
                string title = $"ID {r.TargetId}";
                if (r.TargetType == "place")
                {
                    var place = await _db.Places.FindAsync(r.TargetId);
                    if (place != null) title = $"Địa điểm: {place.Name}";
                }
                else if (r.TargetType == "review")
                {
                    var rev = await _db.Reviews.Include(x => x.Place).FirstOrDefaultAsync(x => x.Id == r.TargetId);
                    if (rev != null)
                    {
                        var preview = string.IsNullOrEmpty(rev.Content) ? "" : (rev.Content.Length > 30 ? rev.Content.Substring(0, 30) + "..." : rev.Content);
                        title = $"Đánh giá tại {rev.Place?.Name}: \"{preview}\"";
                    }
                }
                else if (r.TargetType == "comment")
                {
                    var cmt = await _db.Comments.FindAsync(r.TargetId);
                    if (cmt != null)
                    {
                        var preview = string.IsNullOrEmpty(cmt.Content) ? "" : (cmt.Content.Length > 30 ? cmt.Content.Substring(0, 30) + "..." : cmt.Content);
                        title = $"Bình luận: \"{preview}\"";
                    }
                }

                result.Add(new ReportDto
                {
                    Id = r.Id,
                    ReporterId = r.ReporterId,
                    ReporterName = r.Reporter?.FullName ?? "",
                    TargetType = r.TargetType,
                    TargetId = r.TargetId,
                    TargetTitle = title,
                    ReasonContent = r.Reason?.Content ?? "",
                    Description = r.Description,
                    Status = r.Status,
                    SubmittedAt = r.SubmittedAt
                });
            }

            return result;
        }

        public async Task<bool> ResolveReportAsync(long reportId, long adminUserId, bool confirmViolation, string? note = null)
        {
            var report = await _db.Reports.FindAsync(reportId);
            if (report == null) return false;

            report.Status = "resolved";
            report.Result = confirmViolation ? "violation_confirmed" : "dismissed";
            report.HandledBy = adminUserId;
            report.HandledAt = DateTime.UtcNow;

            if (confirmViolation)
            {
                // Soft delete / Hide content
                if (report.TargetType == "review")
                {
                    var rev = await _db.Reviews.FindAsync(report.TargetId);
                    if (rev != null)
                    {
                        rev.Status = "hidden";
                        await _db.SaveChangesAsync();
                        // Recalculate place rating after hiding review!
                        await _placeService.RecalculatePlaceRatingAsync(rev.PlaceId);

                        _db.Notifications.Add(new Notification
                        {
                            UserId = rev.UserId,
                            Content = "Bài đánh giá của bạn đã bị ẩn do vi phạm tiêu chuẩn cộng đồng.",
                            Type = "review_report_handled",
                            TargetType = "review",
                            TargetId = rev.Id
                        });
                    }
                }
                else if (report.TargetType == "place")
                {
                    var p = await _db.Places.FindAsync(report.TargetId);
                    if (p != null)
                    {
                        p.Status = "hidden";
                        await _db.SaveChangesAsync();
                    }
                }
                else if (report.TargetType == "comment")
                {
                    var c = await _db.Comments.FindAsync(report.TargetId);
                    if (c != null)
                    {
                        c.Status = "hidden";
                        await _db.SaveChangesAsync();
                    }
                }
            }

            _db.Notifications.Add(new Notification
            {
                UserId = report.ReporterId,
                Content = $"Báo cáo vi phạm của bạn đã được kiểm duyệt và xử lý: {(confirmViolation ? "Xác nhận vi phạm & đã xử lý" : "Nội dung hợp lệ / Bác bỏ báo cáo")}.",
                Type = "review_report_handled",
                TargetType = report.TargetType,
                TargetId = report.TargetId
            });

            await AddAuditLogAsync(adminUserId, "RESOLVE_REPORT", report.TargetType, report.TargetId,
                $"Xử lý báo cáo #{reportId}: {(confirmViolation ? "Xác nhận vi phạm" : "Bác bỏ")}. {note}");

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<AppealDto>> GetAppealsAsync(long adminUserId, bool isSystemAdmin = false)
        {
            var query = _db.Appeals
                .Include(a => a.User)
                .AsQueryable();

            if (isSystemAdmin)
            {
                query = query.Where(a => a.Status == "escalated_to_system_admin" || a.Status == "pending");
            }
            else
            {
                var catIds = await GetAdminCategoryIds(adminUserId, false);
                query = query.Where(a => a.Status == "pending" && (
                    (a.TargetType == "place" && _db.Places.Any(p => p.Id == a.TargetId && catIds.Contains(p.CategoryId))) ||
                    (a.TargetType == "review" && _db.Reviews.Any(rev => rev.Id == a.TargetId && _db.Places.Any(p => p.Id == rev.PlaceId && catIds.Contains(p.CategoryId)))) ||
                    (a.TargetType == "comment" && _db.Comments.Any(c => c.Id == a.TargetId && _db.Reviews.Any(rev => rev.Id == c.ReviewId && _db.Places.Any(p => p.Id == rev.PlaceId && catIds.Contains(p.CategoryId))))) ||
                    (a.TargetType == "place_edit_proposal" && _db.PlaceEditProposals.Any(pep => pep.Id == a.TargetId && _db.Places.Any(p => p.Id == pep.PlaceId && catIds.Contains(p.CategoryId))))
                ));
            }

            var list = await query.OrderByDescending(a => a.SubmittedAt).ToListAsync();

            return list.Select(a => new AppealDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.User?.FullName ?? "",
                TargetType = a.TargetType,
                TargetId = a.TargetId,
                Reason = a.Reason,
                Status = a.Status,
                SubmittedAt = a.SubmittedAt,
                CategoryAdminResult = a.CategoryAdminResult,
                FinalResult = a.FinalResult
            }).ToList();
        }

        public async Task<bool> HandleAppealAsync(long appealId, long adminUserId, bool isSystemAdmin, string result, bool escalate = false)
        {
            var appeal = await _db.Appeals.FindAsync(appealId);
            if (appeal == null) return false;

            if (isSystemAdmin)
            {
                appeal.SystemAdminId = adminUserId;
                appeal.FinalResult = result;
                appeal.SystemAdminAt = DateTime.UtcNow;
                appeal.Status = "resolved";
            }
            else
            {
                appeal.CategoryAdminId = adminUserId;
                appeal.CategoryAdminResult = result;
                appeal.CategoryAdminAt = DateTime.UtcNow;

                if (escalate)
                {
                    appeal.Status = "escalated_to_system_admin";
                }
                else
                {
                    appeal.Status = "handled_by_category_admin";
                }
            }

            _db.Notifications.Add(new Notification
            {
                UserId = appeal.UserId,
                Content = $"Khiếu nại của bạn về {appeal.TargetType} #{appeal.TargetId} đã được phản hồi: {result}",
                Type = "appeal_resolved",
                TargetType = appeal.TargetType,
                TargetId = appeal.TargetId
            });

            await AddAuditLogAsync(adminUserId, isSystemAdmin ? "SYSTEM_RESOLVE_APPEAL" : "CAT_HANDLE_APPEAL",
                appeal.TargetType, appeal.TargetId, $"Xử lý khiếu nại #{appealId}: {result}");

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _db.Users
                .Include(u => u.CategoryAssignments)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                AvatarUrl = u.AvatarUrl,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                AssignedCategoryIds = u.CategoryAssignments.Select(c => c.CategoryId).ToList()
            }).ToList();
        }

        public async Task<bool> ToggleUserStatusAsync(long targetUserId, long adminUserId)
        {
            var user = await _db.Users.FindAsync(targetUserId);
            if (user == null || user.Role == "system_admin") return false;

            user.Status = user.Status == "active" ? "locked" : "active";
            await AddAuditLogAsync(adminUserId, "TOGGLE_USER_STATUS", "user", targetUserId, $"Chuyển trạng thái user {user.Email} sang: {user.Status}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(long targetUserId, string newRole, long adminUserId)
        {
            var user = await _db.Users.FindAsync(targetUserId);
            if (user == null || user.Id == adminUserId) return false;

            user.Role = newRole;
            await AddAuditLogAsync(adminUserId, "UPDATE_USER_ROLE", "user", targetUserId, $"Cập nhật vai trò {user.Email} thành: {newRole}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignCategoriesToAdminAsync(long categoryAdminId, List<int> categoryIds, long adminUserId)
        {
            var existing = await _db.AdminCategoryAssignments.Where(a => a.UserId == categoryAdminId).ToListAsync();
            _db.AdminCategoryAssignments.RemoveRange(existing);

            foreach (var cid in categoryIds)
            {
                _db.AdminCategoryAssignments.Add(new AdminCategoryAssignment
                {
                    UserId = categoryAdminId,
                    CategoryId = cid,
                    AssignedAt = DateTime.UtcNow
                });
            }

            await AddAuditLogAsync(adminUserId, "ASSIGN_ADMIN_CATEGORIES", "user", categoryAdminId, $"Phân quyền quản lý {categoryIds.Count} danh mục cho Admin #{categoryAdminId}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<SystemConfig>> GetSystemConfigsAsync()
        {
            return await _db.SystemConfigs.ToListAsync();
        }

        public async Task<bool> UpdateSystemConfigAsync(string key, string value, long adminUserId)
        {
            var cfg = await _db.SystemConfigs.FirstOrDefaultAsync(c => c.ConfigKey == key);
            if (cfg != null)
            {
                cfg.ConfigValue = value;
                cfg.UpdatedAt = DateTime.UtcNow;
                cfg.UpdatedBy = adminUserId;
            }
            else
            {
                _db.SystemConfigs.Add(new SystemConfig
                {
                    ConfigKey = key,
                    ConfigValue = value,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = adminUserId
                });
            }

            await AddAuditLogAsync(adminUserId, "UPDATE_SYSTEM_CONFIG", "system_config", null, $"Cập nhật cấu hình '{key}' = '{value}'");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<AuditLogDto>> GetAuditLogsAsync(int limit = 50)
        {
            var logs = await _db.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();

            return logs.Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserName = a.User?.FullName ?? "Hệ thống",
                Action = a.Action,
                TargetType = a.TargetType,
                TargetId = a.TargetId,
                Description = a.Description,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task AddAuditLogAsync(long userId, string action, string? targetType, long? targetId, string? desc)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                Description = desc,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<DashboardStatsDto> GetAdminDashboardStatsAsync(long adminUserId, bool isSystemAdmin = false)
        {
            var catIds = await GetAdminCategoryIds(adminUserId, isSystemAdmin);
            
            var placesQuery = _db.Places.AsQueryable();
            if (!isSystemAdmin) placesQuery = placesQuery.Where(p => catIds.Contains(p.CategoryId));
            
            var totalPlaces = await placesQuery.CountAsync(p => p.Status == "approved");
            var pendingPlaces = await placesQuery.CountAsync(p => p.Status == "pending");
            
            var reviewsQuery = _db.Reviews.AsQueryable();
            if (!isSystemAdmin) reviewsQuery = reviewsQuery.Where(r => _db.Places.Any(p => p.Id == r.PlaceId && catIds.Contains(p.CategoryId)));
            var totalReviews = await reviewsQuery.CountAsync();
            
            var visitsQuery = _db.VisitLogs.AsQueryable();
            var favsQuery = _db.Favorites.AsQueryable();
            if (!isSystemAdmin) {
                visitsQuery = visitsQuery.Where(v => _db.Places.Any(p => p.Id == v.PlaceId && catIds.Contains(p.CategoryId)));
                favsQuery = favsQuery.Where(f => _db.Places.Any(p => p.Id == f.PlaceId && catIds.Contains(p.CategoryId)));
            }
            var interactions = await visitsQuery.CountAsync() + await favsQuery.CountAsync();
            
            var reportsQuery = _db.Reports.AsQueryable();
            if (!isSystemAdmin) {
                reportsQuery = reportsQuery.Where(r => 
                    (r.TargetType == "place" && _db.Places.Any(p => p.Id == r.TargetId && catIds.Contains(p.CategoryId))) ||
                    (r.TargetType == "review" && _db.Reviews.Any(rev => rev.Id == r.TargetId && _db.Places.Any(p => p.Id == rev.PlaceId && catIds.Contains(p.CategoryId)))) ||
                    (r.TargetType == "comment" && _db.Comments.Any(c => c.Id == r.TargetId && _db.Reviews.Any(rev => rev.Id == c.ReviewId && _db.Places.Any(p => p.Id == rev.PlaceId && catIds.Contains(p.CategoryId)))))
                );
            }
            var pendingReports = await reportsQuery.CountAsync(r => r.Status == "pending");
            
            return new DashboardStatsDto {
                TotalPlaces = totalPlaces,
                PendingPlaces = pendingPlaces,
                TotalReviews = totalReviews,
                UserInteractions = interactions,
                PendingReports = pendingReports
            };
        }
    }
}
