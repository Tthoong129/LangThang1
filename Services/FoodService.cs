using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniMap.Data;
using MiniMap.Models;

namespace MiniMap.Services
{
    public interface IFoodService
    {
        Task<List<FoodDto>> GetFoodsByProvinceAsync(int provinceId);
        Task<List<FoodDto>> GetAllFoodsAsync();
        Task<FoodDetailDto?> GetFoodDetailAsync(long foodId);
    }

    public class FoodDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Provinces { get; set; } = new();
        public int PlaceCount { get; set; }
    }

    public class FoodDetailDto : FoodDto
    {
        public List<PlaceDto> PlacesServing { get; set; } = new();
    }

    public class FoodService : IFoodService
    {
        private readonly TravelReviewDbContext _db;

        public FoodService(TravelReviewDbContext db)
        {
            _db = db;
        }

        public async Task<List<FoodDto>> GetFoodsByProvinceAsync(int provinceId)
        {
            var foods = await _db.Foods
                .Include(f => f.FoodProvinces).ThenInclude(fp => fp.Province)
                .Include(f => f.FoodPlaces)
                .Where(f => f.FoodProvinces.Any(fp => fp.ProvinceId == provinceId))
                .ToListAsync();

            return foods.Select(f => new FoodDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                Provinces = f.FoodProvinces.Select(fp => fp.Province?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList(),
                PlaceCount = f.FoodPlaces.Count
            }).ToList();
        }

        public async Task<List<FoodDto>> GetAllFoodsAsync()
        {
            var foods = await _db.Foods
                .Include(f => f.FoodProvinces).ThenInclude(fp => fp.Province)
                .Include(f => f.FoodPlaces)
                .ToListAsync();

            return foods.Select(f => new FoodDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                Provinces = f.FoodProvinces.Select(fp => fp.Province?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList(),
                PlaceCount = f.FoodPlaces.Count
            }).ToList();
        }

        public async Task<FoodDetailDto?> GetFoodDetailAsync(long foodId)
        {
            var f = await _db.Foods
                .Include(f => f.FoodProvinces).ThenInclude(fp => fp.Province)
                .Include(f => f.FoodPlaces).ThenInclude(fp => fp.Place).ThenInclude(p => p!.Province)
                .Include(f => f.FoodPlaces).ThenInclude(fp => fp.Place).ThenInclude(p => p!.Category)
                .Include(f => f.FoodPlaces).ThenInclude(fp => fp.Place).ThenInclude(p => p!.Media)
                .FirstOrDefaultAsync(f => f.Id == foodId);

            if (f == null) return null;

            return new FoodDetailDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                Provinces = f.FoodProvinces.Select(fp => fp.Province?.Name ?? "").ToList(),
                PlaceCount = f.FoodPlaces.Count,
                PlacesServing = f.FoodPlaces.Where(fp => fp.Place != null && fp.Place.Status == "approved").Select(fp => new PlaceDto
                {
                    Id = fp.Place!.Id,
                    Name = fp.Place.Name,
                    Description = fp.Place.Description,
                    Address = fp.Place.Address,
                    MinPrice = fp.Place.MinPrice,
                    MaxPrice = fp.Place.MaxPrice,
                    AvgRating = fp.Place.AvgRating,
                    ReviewCount = fp.Place.ReviewCount,
                    ProvinceName = fp.Place.Province?.Name ?? "",
                    CategoryName = fp.Place.Category?.Name ?? "",
                    ThumbnailUrl = fp.Place.Media.FirstOrDefault(m => m.MediaType == "image")?.Url ?? "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500",
                    Latitude = fp.Place.Latitude,
                    Longitude = fp.Place.Longitude
                }).ToList()
            };
        }
    }
}
