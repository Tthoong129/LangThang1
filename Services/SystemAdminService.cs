using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniMap.Data;
using MiniMap.Models;

namespace MiniMap.Services
{
    public interface ISystemAdminService
    {
        Task<SystemDashboardStatsDto> GetSystemDashboardStatsAsync();
        
        // Regions
        Task<List<Region>> GetRegionsAsync();
        Task<Region> CreateRegionAsync(string name);
        Task<Region?> UpdateRegionAsync(int id, string name, string status);
        
        // Provinces
        Task<List<ProvinceDto>> GetProvincesAsync();
        Task<Province> CreateProvinceAsync(string name, int regionId);
        Task<Province?> UpdateProvinceAsync(int id, string name, int regionId, string status);
        
        // PlaceTypes
        Task<List<PlaceType>> GetPlaceTypesAsync();
        Task<PlaceType> CreatePlaceTypeAsync(string name);
        Task<PlaceType?> UpdatePlaceTypeAsync(int id, string name, string status);
        
        // Categories
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<Category> CreateCategoryAsync(string name, int placeTypeId);
        Task<Category?> UpdateCategoryAsync(int id, string name, int placeTypeId, string status);
        
        // ReportReasons
        Task<List<ReportReason>> GetReportReasonsAsync();
        Task<ReportReason> CreateReportReasonAsync(string content);
        Task<ReportReason?> UpdateReportReasonAsync(int id, string content, string status);
        
        // Foods
        Task<List<Food>> GetFoodsAsync();
        Task<Food> CreateFoodAsync(string name, string? description, string? imageUrl);
        Task<Food?> UpdateFoodAsync(long id, string name, string? description, string? imageUrl);
        Task<bool> DeleteFoodAsync(long id);
        
        // FoodPlaces
        Task<List<FoodPlaceDto>> GetFoodPlacesAsync();
        Task<bool> AddFoodPlaceAsync(long foodId, long placeId);
        Task<bool> RemoveFoodPlaceAsync(long foodId, long placeId);
    }

    public class SystemDashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCategoryAdmins { get; set; }
        public int TotalPlaces { get; set; }
        public int PendingPlaces { get; set; }
        public int VisibleReviews { get; set; }
        public int VisibleComments { get; set; }
        public int PendingReports { get; set; }
        public int PendingAppeals { get; set; }
    }

    public class ProvinceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PlaceTypeId { get; set; }
        public string PlaceTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class FoodPlaceDto
    {
        public long FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public long PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
    }

    public class SystemAdminService : ISystemAdminService
    {
        private readonly TravelReviewDbContext _db;

        public SystemAdminService(TravelReviewDbContext db)
        {
            _db = db;
        }

        public async Task<SystemDashboardStatsDto> GetSystemDashboardStatsAsync()
        {
            return new SystemDashboardStatsDto
            {
                TotalUsers = await _db.Users.CountAsync(),
                TotalCategoryAdmins = await _db.Users.CountAsync(u => u.Role == "category_admin"),
                TotalPlaces = await _db.Places.CountAsync(p => p.Status == "active"),
                PendingPlaces = await _db.PlaceProposals.CountAsync(p => p.Status == "pending"),
                VisibleReviews = await _db.Reviews.CountAsync(r => r.Status == "visible"),
                VisibleComments = await _db.Comments.CountAsync(c => c.Status == "visible"),
                PendingReports = await _db.Reports.CountAsync(r => r.Status == "pending"),
                PendingAppeals = await _db.Appeals.CountAsync(a => a.Status == "pending" || a.Status == "escalated_to_system_admin")
            };
        }

        public async Task<List<Region>> GetRegionsAsync() => await _db.Regions.ToListAsync();
        public async Task<Region> CreateRegionAsync(string name)
        {
            var r = new Region { Name = name, Status = "active" };
            _db.Regions.Add(r);
            await _db.SaveChangesAsync();
            return r;
        }
        public async Task<Region?> UpdateRegionAsync(int id, string name, string status)
        {
            var r = await _db.Regions.FindAsync(id);
            if (r == null) return null;
            r.Name = name;
            r.Status = status;
            await _db.SaveChangesAsync();
            return r;
        }

        public async Task<List<ProvinceDto>> GetProvincesAsync()
        {
            return await _db.Provinces.Include(p => p.Region).Select(p => new ProvinceDto
            {
                Id = p.Id,
                Name = p.Name,
                RegionId = p.RegionId,
                RegionName = p.Region!.Name,
                Status = p.Status
            }).ToListAsync();
        }
        public async Task<Province> CreateProvinceAsync(string name, int regionId)
        {
            var p = new Province { Name = name, RegionId = regionId, Status = "active" };
            _db.Provinces.Add(p);
            await _db.SaveChangesAsync();
            return p;
        }
        public async Task<Province?> UpdateProvinceAsync(int id, string name, int regionId, string status)
        {
            var p = await _db.Provinces.FindAsync(id);
            if (p == null) return null;
            p.Name = name;
            p.RegionId = regionId;
            p.Status = status;
            await _db.SaveChangesAsync();
            return p;
        }

        public async Task<List<PlaceType>> GetPlaceTypesAsync() => await _db.PlaceTypes.ToListAsync();
        public async Task<PlaceType> CreatePlaceTypeAsync(string name)
        {
            var t = new PlaceType { Name = name, Status = "active" };
            _db.PlaceTypes.Add(t);
            await _db.SaveChangesAsync();
            return t;
        }
        public async Task<PlaceType?> UpdatePlaceTypeAsync(int id, string name, string status)
        {
            var t = await _db.PlaceTypes.FindAsync(id);
            if (t == null) return null;
            t.Name = name;
            t.Status = status;
            await _db.SaveChangesAsync();
            return t;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _db.Categories.Include(c => c.PlaceType).Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                PlaceTypeId = c.PlaceTypeId,
                PlaceTypeName = c.PlaceType!.Name,
                Status = c.Status
            }).ToListAsync();
        }
        public async Task<Category> CreateCategoryAsync(string name, int placeTypeId)
        {
            var c = new Category { Name = name, PlaceTypeId = placeTypeId, Status = "active" };
            _db.Categories.Add(c);
            await _db.SaveChangesAsync();
            return c;
        }
        public async Task<Category?> UpdateCategoryAsync(int id, string name, int placeTypeId, string status)
        {
            var c = await _db.Categories.FindAsync(id);
            if (c == null) return null;
            c.Name = name;
            c.PlaceTypeId = placeTypeId;
            c.Status = status;
            await _db.SaveChangesAsync();
            return c;
        }

        public async Task<List<ReportReason>> GetReportReasonsAsync() => await _db.ReportReasons.ToListAsync();
        public async Task<ReportReason> CreateReportReasonAsync(string content)
        {
            var r = new ReportReason { Content = content, Status = "active" };
            _db.ReportReasons.Add(r);
            await _db.SaveChangesAsync();
            return r;
        }
        public async Task<ReportReason?> UpdateReportReasonAsync(int id, string content, string status)
        {
            var r = await _db.ReportReasons.FindAsync(id);
            if (r == null) return null;
            r.Content = content;
            r.Status = status;
            await _db.SaveChangesAsync();
            return r;
        }

        public async Task<List<Food>> GetFoodsAsync() => await _db.Foods.ToListAsync();
        public async Task<Food> CreateFoodAsync(string name, string? description, string? imageUrl)
        {
            var f = new Food { Name = name, Description = description, ImageUrl = imageUrl, CreatedAt = DateTime.UtcNow };
            _db.Foods.Add(f);
            await _db.SaveChangesAsync();
            return f;
        }
        public async Task<Food?> UpdateFoodAsync(long id, string name, string? description, string? imageUrl)
        {
            var f = await _db.Foods.FindAsync(id);
            if (f == null) return null;
            f.Name = name;
            f.Description = description;
            f.ImageUrl = imageUrl;
            await _db.SaveChangesAsync();
            return f;
        }
        public async Task<bool> DeleteFoodAsync(long id)
        {
            var f = await _db.Foods.FindAsync(id);
            if (f == null) return false;
            _db.Foods.Remove(f);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<FoodPlaceDto>> GetFoodPlacesAsync()
        {
            return await _db.FoodPlaces.Include(f => f.Food).Include(f => f.Place).Select(f => new FoodPlaceDto
            {
                FoodId = f.FoodId,
                FoodName = f.Food!.Name,
                PlaceId = f.PlaceId,
                PlaceName = f.Place!.Name
            }).ToListAsync();
        }
        public async Task<bool> AddFoodPlaceAsync(long foodId, long placeId)
        {
            if (await _db.FoodPlaces.AnyAsync(f => f.FoodId == foodId && f.PlaceId == placeId)) return false;
            _db.FoodPlaces.Add(new FoodPlace { FoodId = foodId, PlaceId = placeId });
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveFoodPlaceAsync(long foodId, long placeId)
        {
            var fp = await _db.FoodPlaces.FirstOrDefaultAsync(f => f.FoodId == foodId && f.PlaceId == placeId);
            if (fp == null) return false;
            _db.FoodPlaces.Remove(fp);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
