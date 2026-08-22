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
    public interface IPlaceService
    {
        Task<List<PlaceDto>> SearchPlacesAsync(PlaceFilterDto filter);
        Task<PlaceDetailDto?> GetPlaceDetailAsync(long id, long? currentUserId = null);
        Task<PlaceProposal> ProposePlaceAsync(ProposePlaceDto dto, long userId);
        Task<PlaceEditProposal> ProposeEditAsync(ProposeEditDto dto, long userId);
        Task RecalculatePlaceRatingAsync(long placeId);
        Task<List<PlaceDto>> GetTopRankedPlacesAsync(int? provinceId = null, int? regionId = null, int? placeTypeId = null, int limit = 10);
        Task RecordAccessHistoryAsync(long placeId, long userId);
        Task<List<MyProposalDto>> GetUserProposalsAsync(long userId);
        Task<bool> UpdateProposalAsync(long proposalId, ProposePlaceDto dto, long userId);
        Task<bool> DeleteProposalAsync(long proposalId, long userId);
        Task<PlaceProposal?> GetProposalDetailAsync(long id, long userId);
        
        Task<List<PlaceEditProposal>> GetUserEditProposalsAsync(long userId);
        Task<bool> UpdateEditProposalAsync(long proposalId, ProposePlaceDto dto, long userId);
        Task<bool> DeleteEditProposalAsync(long proposalId, long userId);
        Task<PlaceEditProposal?> GetEditProposalDetailAsync(long id, long userId);
    }

    public class PlaceFilterDto
    {
        public string? Keyword { get; set; }
        public int? RegionId { get; set; }
        public int? ProvinceId { get; set; }
        public int? PlaceTypeId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? UserLat { get; set; }
        public decimal? UserLng { get; set; }
        public double? RadiusKm { get; set; }
        public string? SortBy { get; set; } // 'rating', 'reviews', 'price_asc', 'price_desc', 'newest'
    }

    public class PlaceDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? OpeningHours { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string PlaceTypeName { get; set; } = string.Empty;
        public decimal AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public string? ThumbnailUrl { get; set; }
        public double? DistanceKm { get; set; }
        public string Status { get; set; } = "approved";
    }

    public class PlaceDetailDto : PlaceDto
    {
        public List<PlaceMediaDto> MediaList { get; set; } = new();
        public List<ReviewDto> Reviews { get; set; } = new();
        public List<FoodSummaryDto> Foods { get; set; } = new();
        public bool IsFavorite { get; set; }
        public bool HasVisited { get; set; }
    }

    public class PlaceMediaDto
    {
        public long Id { get; set; }
        public string MediaType { get; set; } = "image";
        public string Url { get; set; } = string.Empty;
    }

    public class ReviewDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public byte Rating { get; set; }
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExperienceDate { get; set; }
        public List<string> Images { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
    }

    public class CommentDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class FoodSummaryDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ProposePlaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? OpeningHours { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int ProvinceId { get; set; }
        public int CategoryId { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<string>? VideoUrls { get; set; }
    }

    public class ProposeEditDto
    {
        public long PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? OpeningHours { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int ProvinceId { get; set; }
        public int CategoryId { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<string>? VideoUrls { get; set; }
    }

    public class MyProposalDto
    {
        public long Id { get; set; }
        public string ProposalType { get; set; } = "create"; // 'create','edit'
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public string? RejectReason { get; set; }
        public DateTime SubmittedAt { get; set; }
        public long? ApprovedPlaceId { get; set; }
        public long? TargetPlaceId { get; set; }
    }

    public class PlaceService : IPlaceService
    {
        private readonly TravelReviewDbContext _db;

        public PlaceService(TravelReviewDbContext db)
        {
            _db = db;
        }

        public async Task<List<PlaceDto>> SearchPlacesAsync(PlaceFilterDto filter)
        {
            var query = _db.Places
                .Include(p => p.Province).ThenInclude(pr => pr!.Region)
                .Include(p => p.Category).ThenInclude(c => c!.PlaceType)
                .Include(p => p.Media)
                .Where(p => p.Status == "active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(kw) ||
                                         (p.Description != null && p.Description.ToLower().Contains(kw)) ||
                                         p.Address.ToLower().Contains(kw));
            }

            if (filter.ProvinceId.HasValue && filter.ProvinceId > 0)
            {
                query = query.Where(p => p.ProvinceId == filter.ProvinceId.Value);
            }
            else if (filter.RegionId.HasValue && filter.RegionId > 0)
            {
                query = query.Where(p => p.Province != null && p.Province.RegionId == filter.RegionId.Value);
            }

            if (filter.CategoryId.HasValue && filter.CategoryId > 0)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }
            else if (filter.PlaceTypeId.HasValue && filter.PlaceTypeId > 0)
            {
                query = query.Where(p => p.Category != null && p.Category.PlaceTypeId == filter.PlaceTypeId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.MaxPrice == null || p.MaxPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.MinPrice == null || p.MinPrice <= filter.MaxPrice.Value);
            }

            if (filter.MinRating.HasValue && filter.MinRating > 0)
            {
                query = query.Where(p => p.AvgRating >= filter.MinRating.Value);
            }

            var places = await query.ToListAsync();

            var result = places.Select(p =>
            {
                double? distance = null;
                if (filter.UserLat.HasValue && filter.UserLng.HasValue && p.Latitude.HasValue && p.Longitude.HasValue)
                {
                    distance = CalculateDistance(
                        (double)filter.UserLat.Value, (double)filter.UserLng.Value,
                        (double)p.Latitude.Value, (double)p.Longitude.Value
                    );
                }

                var thumb = p.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ??
                            "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500";

                return new PlaceDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Phone = p.Phone,
                    Website = p.Website,
                    MinPrice = p.MinPrice,
                    MaxPrice = p.MaxPrice,
                    OpeningHours = p.OpeningHours,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.Province?.Name ?? "",
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? "",
                    PlaceTypeName = p.Category?.PlaceType?.Name ?? "",
                    AvgRating = p.AvgRating,
                    ReviewCount = p.ReviewCount,
                    ThumbnailUrl = thumb,
                    DistanceKm = distance.HasValue ? Math.Round(distance.Value, 1) : null,
                    Status = p.Status
                };
            }).ToList();

            if (filter.RadiusKm.HasValue && filter.RadiusKm > 0 && filter.UserLat.HasValue && filter.UserLng.HasValue)
            {
                result = result.Where(r => r.DistanceKm.HasValue && r.DistanceKm.Value <= filter.RadiusKm.Value).ToList();
            }

            // Sorting
            switch (filter.SortBy)
            {
                case "rating":
                    result = result.OrderByDescending(r => r.AvgRating).ThenByDescending(r => r.ReviewCount).ToList();
                    break;
                case "reviews":
                    result = result.OrderByDescending(r => r.ReviewCount).ToList();
                    break;
                case "price_asc":
                    result = result.OrderBy(r => r.MinPrice ?? decimal.MaxValue).ToList();
                    break;
                case "price_desc":
                    result = result.OrderByDescending(r => r.MaxPrice ?? decimal.MinValue).ToList();
                    break;
                case "distance":
                    result = result.OrderBy(r => r.DistanceKm ?? double.MaxValue).ToList();
                    break;
                default:
                    result = result.OrderByDescending(r => r.Id).ToList();
                    break;
            }

            return result;
        }

        public async Task<PlaceDetailDto?> GetPlaceDetailAsync(long id, long? currentUserId = null)
        {
            var p = await _db.Places
                .Include(p => p.Province).ThenInclude(pr => pr!.Region)
                .Include(p => p.Category).ThenInclude(c => c!.PlaceType)
                .Include(p => p.Media)
                .Include(p => p.Reviews.Where(r => r.Status == "visible"))
                    .ThenInclude(r => r.User)
                .Include(p => p.Reviews.Where(r => r.Status == "visible"))
                    .ThenInclude(r => r.Media)
                .Include(p => p.Reviews.Where(r => r.Status == "visible"))
                    .ThenInclude(r => r.Comments.Where(c => c.Status == "visible"))
                        .ThenInclude(c => c.User)
                .Include(p => p.FoodPlaces)
                    .ThenInclude(fp => fp.Food)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return null;

            // Auto record access history if user logged in
            if (currentUserId.HasValue && currentUserId.Value > 0)
            {
                var existingHistory = await _db.AccessHistories
                    .FirstOrDefaultAsync(h => h.UserId == currentUserId.Value && h.PlaceId == id);
                if (existingHistory != null)
                {
                    existingHistory.ViewedAt = DateTime.UtcNow;
                }
                else
                {
                    _db.AccessHistories.Add(new AccessHistory
                    {
                        UserId = currentUserId.Value,
                        PlaceId = id,
                        ViewedAt = DateTime.UtcNow
                    });
                }
                await _db.SaveChangesAsync();
            }

            bool isFav = false;
            bool hasVisited = false;
            if (currentUserId.HasValue && currentUserId.Value > 0)
            {
                isFav = await _db.Favorites.AnyAsync(f => f.UserId == currentUserId.Value && f.PlaceId == id);
                hasVisited = await _db.VisitLogs.AnyAsync(v => v.UserId == currentUserId.Value && v.PlaceId == id);
            }

            var thumb = p.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ??
                        "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500";

            return new PlaceDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Address = p.Address,
                Phone = p.Phone,
                Website = p.Website,
                MinPrice = p.MinPrice,
                MaxPrice = p.MaxPrice,
                OpeningHours = p.OpeningHours,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ProvinceId = p.ProvinceId,
                ProvinceName = p.Province?.Name ?? "",
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "",
                PlaceTypeName = p.Category?.PlaceType?.Name ?? "",
                AvgRating = p.AvgRating,
                ReviewCount = p.ReviewCount,
                ThumbnailUrl = thumb,
                Status = p.Status,
                IsFavorite = isFav,
                HasVisited = hasVisited,
                MediaList = p.Media.Select(m => new PlaceMediaDto
                {
                    Id = m.Id,
                    MediaType = m.MediaType,
                    Url = m.Url
                }).ToList(),
                Reviews = p.Reviews.OrderByDescending(r => r.CreatedAt).Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User?.FullName ?? "Người dùng",
                    UserAvatar = r.User?.AvatarUrl,
                    Rating = r.Rating,
                    Content = r.Content,
                    VideoUrl = r.VideoUrl,
                    CreatedAt = r.CreatedAt,
                    ExperienceDate = r.ExperienceDate,
                    Images = r.Media.Select(m => m.ImageUrl).ToList(),
                    Comments = r.Comments.OrderBy(c => c.CreatedAt).Select(c => new CommentDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User?.FullName ?? "Người dùng",
                        UserAvatar = c.User?.AvatarUrl,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }).ToList()
                }).ToList(),
                Foods = p.FoodPlaces.Where(fp => fp.Food != null).Select(fp => new FoodSummaryDto
                {
                    Id = fp.Food!.Id,
                    Name = fp.Food.Name,
                    Description = fp.Food.Description,
                    ImageUrl = fp.Food.ImageUrl
                }).ToList()
            };
        }

        public async Task<PlaceProposal> ProposePlaceAsync(ProposePlaceDto dto, long userId)
        {
            var proposal = new PlaceProposal
            {
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                Phone = dto.Phone,
                Website = dto.Website,
                MinPrice = dto.MinPrice,
                MaxPrice = dto.MaxPrice,
                OpeningHours = dto.OpeningHours,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ProvinceId = dto.ProvinceId,
                CategoryId = dto.CategoryId,
                ProposedBy = userId,
                SubmittedAt = DateTime.UtcNow,
                Status = "pending"
            };

            _db.PlaceProposals.Add(proposal);
            await _db.SaveChangesAsync();

            if (dto.ImageUrls != null)
            {
                foreach (var img in dto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.ProposalMedia.Add(new ProposalMedia
                    {
                        ProposalId = proposal.Id,
                        MediaType = "image",
                        Url = img
                    });
                }
            }

            if (dto.VideoUrls != null)
            {
                foreach (var vid in dto.VideoUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.ProposalMedia.Add(new ProposalMedia
                    {
                        ProposalId = proposal.Id,
                        MediaType = "video",
                        Url = vid
                    });
                }
            }

            await _db.SaveChangesAsync();
            return proposal;
        }

        public async Task<PlaceEditProposal> ProposeEditAsync(ProposeEditDto dto, long userId)
        {
            var proposal = new PlaceEditProposal
            {
                PlaceId = dto.PlaceId,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                Phone = dto.Phone,
                Website = dto.Website,
                MinPrice = dto.MinPrice,
                MaxPrice = dto.MaxPrice,
                OpeningHours = dto.OpeningHours,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ProvinceId = dto.ProvinceId,
                CategoryId = dto.CategoryId,
                ProposedBy = userId,
                Status = "pending",
                SubmittedAt = DateTime.UtcNow
            };
 
            _db.PlaceEditProposals.Add(proposal);
            await _db.SaveChangesAsync();

            // Save media for edit proposals
            if (dto.ImageUrls != null)
            {
                foreach (var img in dto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.PlaceEditProposalMedia.Add(new PlaceEditProposalMedia
                    {
                        PlaceEditProposalId = proposal.Id,
                        MediaType = "image",
                        Url = img
                    });
                }
            }

            if (dto.VideoUrls != null)
            {
                foreach (var vid in dto.VideoUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.PlaceEditProposalMedia.Add(new PlaceEditProposalMedia
                    {
                        PlaceEditProposalId = proposal.Id,
                        MediaType = "video",
                        Url = vid
                    });
                }
            }

            await _db.SaveChangesAsync();
            return proposal;
        }

        public async Task RecalculatePlaceRatingAsync(long placeId)
        {
            var activeReviews = await _db.Reviews
                .Where(r => r.PlaceId == placeId && r.Status == "visible")
                .ToListAsync();

            var place = await _db.Places.FindAsync(placeId);
            if (place != null)
            {
                if (activeReviews.Any())
                {
                    place.ReviewCount = activeReviews.Count;
                    place.AvgRating = Math.Round((decimal)activeReviews.Average(r => r.Rating), 1);
                }
                else
                {
                    place.ReviewCount = 0;
                    place.AvgRating = 0;
                }
                place.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<PlaceDto>> GetTopRankedPlacesAsync(int? provinceId = null, int? regionId = null, int? placeTypeId = null, int limit = 10)
        {
            var query = _db.Places
                .Include(p => p.Province).ThenInclude(pr => pr!.Region)
                .Include(p => p.Category).ThenInclude(c => c!.PlaceType)
                .Include(p => p.Media)
                .Where(p => p.Status == "active")
                .AsQueryable();

            if (provinceId.HasValue && provinceId > 0)
                query = query.Where(p => p.ProvinceId == provinceId.Value);
            else if (regionId.HasValue && regionId > 0)
                query = query.Where(p => p.Province != null && p.Province.RegionId == regionId.Value);

            if (placeTypeId.HasValue && placeTypeId > 0)
                query = query.Where(p => p.Category != null && p.Category.PlaceTypeId == placeTypeId.Value);

            var places = await query
                .OrderByDescending(p => p.AvgRating)
                .ThenByDescending(p => p.ReviewCount)
                .Take(limit)
                .ToListAsync();

            return places.Select(p => new PlaceDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Address = p.Address,
                AvgRating = p.AvgRating,
                ReviewCount = p.ReviewCount,
                ProvinceName = p.Province?.Name ?? "",
                CategoryName = p.Category?.Name ?? "",
                PlaceTypeName = p.Category?.PlaceType?.Name ?? "",
                ThumbnailUrl = p.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ?? "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500",
                Latitude = p.Latitude,
                Longitude = p.Longitude
            }).ToList();
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var rlat1 = Math.PI * lat1 / 180.0;
            var rlat2 = Math.PI * lat2 / 180.0;
            var theta = lon1 - lon2;
            var rtheta = Math.PI * theta / 180.0;
            var dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(Math.Min(dist, 1.0));
            dist = dist * 180.0 / Math.PI;
            dist = dist * 60.0 * 1.1515 * 1.609344; // to Km
            return dist;
        }
        public async Task RecordAccessHistoryAsync(long placeId, long userId)
        {
            // Upsert: remove old entry for same place+user (keep history clean)
            var existing = await _db.AccessHistories
                .FirstOrDefaultAsync(a => a.PlaceId == placeId && a.UserId == userId);
            if (existing != null)
            {
                existing.ViewedAt = DateTime.UtcNow;
            }
            else
            {
                _db.AccessHistories.Add(new AccessHistory
                {
                    PlaceId = placeId,
                    UserId = userId,
                    ViewedAt = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync();
        }

        public async Task<List<MyProposalDto>> GetUserProposalsAsync(long userId)
        {
            var creations = await _db.PlaceProposals
                .Where(p => p.ProposedBy == userId)
                .Select(p => new MyProposalDto
                {
                    Id = p.Id,
                    ProposalType = "create",
                    Name = p.Name,
                    Address = p.Address,
                    Status = p.Status,
                    RejectReason = p.RejectReason,
                    SubmittedAt = p.SubmittedAt,
                    ApprovedPlaceId = p.ApprovedPlaceId,
                    TargetPlaceId = null
                })
                .ToListAsync();

            var edits = await _db.PlaceEditProposals
                .Where(p => p.ProposedBy == userId)
                .Select(p => new MyProposalDto
                {
                    Id = p.Id,
                    ProposalType = "edit",
                    Name = p.Name,
                    Address = p.Address,
                    Status = p.Status,
                    RejectReason = p.RejectReason,
                    SubmittedAt = p.SubmittedAt,
                    ApprovedPlaceId = null,
                    TargetPlaceId = p.PlaceId
                })
                .ToListAsync();

            var combined = creations.Concat(edits)
                .OrderByDescending(p => p.SubmittedAt)
                .ToList();

            return combined;
        }

        public async Task<bool> UpdateProposalAsync(long proposalId, ProposePlaceDto dto, long userId)
        {
            var proposal = await _db.PlaceProposals
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.ProposedBy == userId);
            
            if (proposal == null) return false;
            if (proposal.Status != "pending" && proposal.Status != "rejected") return false;

            // Update details
            proposal.Name = dto.Name;
            proposal.Description = dto.Description;
            proposal.Address = dto.Address;
            proposal.Phone = dto.Phone;
            proposal.Website = dto.Website;
            proposal.MinPrice = dto.MinPrice;
            proposal.MaxPrice = dto.MaxPrice;
            proposal.OpeningHours = dto.OpeningHours;
            proposal.Latitude = dto.Latitude;
            proposal.Longitude = dto.Longitude;
            proposal.ProvinceId = dto.ProvinceId;
            proposal.CategoryId = dto.CategoryId;
            
            // If it was rejected, resubmitting turns status back to pending
            proposal.Status = "pending";
            proposal.RejectReason = null;
            proposal.SubmittedAt = DateTime.UtcNow;

            // Remove old media
            _db.ProposalMedia.RemoveRange(proposal.Media);

            // Add new media
            if (dto.ImageUrls != null)
            {
                foreach (var img in dto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.ProposalMedia.Add(new ProposalMedia
                    {
                        ProposalId = proposal.Id,
                        MediaType = "image",
                        Url = img
                    });
                }
            }

            if (dto.VideoUrls != null)
            {
                foreach (var vid in dto.VideoUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.ProposalMedia.Add(new ProposalMedia
                    {
                        ProposalId = proposal.Id,
                        MediaType = "video",
                        Url = vid
                    });
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProposalAsync(long proposalId, long userId)
        {
            var proposal = await _db.PlaceProposals
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.ProposedBy == userId);

            if (proposal == null) return false;
            // Only allow deleting pending or rejected proposals
            if (proposal.Status != "pending" && proposal.Status != "rejected") return false;

            _db.PlaceProposals.Remove(proposal);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PlaceProposal?> GetProposalDetailAsync(long id, long userId)
        {
            return await _db.PlaceProposals
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == id && p.ProposedBy == userId);
        }

        public async Task<List<PlaceEditProposal>> GetUserEditProposalsAsync(long userId)
        {
            return await _db.PlaceEditProposals
                .Include(p => p.Media)
                .Include(p => p.Place)
                .Where(p => p.ProposedBy == userId)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateEditProposalAsync(long proposalId, ProposePlaceDto dto, long userId)
        {
            var proposal = await _db.PlaceEditProposals
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.ProposedBy == userId);

            if (proposal == null) return false;
            if (proposal.Status != "pending" && proposal.Status != "rejected") return false;

            proposal.Name = dto.Name;
            proposal.Description = dto.Description;
            proposal.Address = dto.Address;
            proposal.Phone = dto.Phone;
            proposal.Website = dto.Website;
            proposal.MinPrice = dto.MinPrice;
            proposal.MaxPrice = dto.MaxPrice;
            proposal.OpeningHours = dto.OpeningHours;
            proposal.Latitude = dto.Latitude;
            proposal.Longitude = dto.Longitude;
            proposal.ProvinceId = dto.ProvinceId;
            proposal.CategoryId = dto.CategoryId;

            proposal.Status = "pending";
            proposal.RejectReason = null;
            proposal.SubmittedAt = DateTime.UtcNow;

            _db.PlaceEditProposalMedia.RemoveRange(proposal.Media);

            if (dto.ImageUrls != null)
            {
                foreach (var img in dto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.PlaceEditProposalMedia.Add(new PlaceEditProposalMedia
                    {
                        PlaceEditProposalId = proposal.Id,
                        MediaType = "image",
                        Url = img
                    });
                }
            }

            if (dto.VideoUrls != null)
            {
                foreach (var vid in dto.VideoUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.PlaceEditProposalMedia.Add(new PlaceEditProposalMedia
                    {
                        PlaceEditProposalId = proposal.Id,
                        MediaType = "video",
                        Url = vid
                    });
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEditProposalAsync(long proposalId, long userId)
        {
            var proposal = await _db.PlaceEditProposals
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.ProposedBy == userId);

            if (proposal == null) return false;
            if (proposal.Status != "pending" && proposal.Status != "rejected") return false;

            _db.PlaceEditProposals.Remove(proposal);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PlaceEditProposal?> GetEditProposalDetailAsync(long id, long userId)
        {
            return await _db.PlaceEditProposals
                .Include(p => p.Media)
                .Include(p => p.Place)
                .FirstOrDefaultAsync(p => p.Id == id && p.ProposedBy == userId);
        }
    }
}
