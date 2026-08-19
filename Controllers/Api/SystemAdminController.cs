using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemAdminController : ControllerBase
    {
        private readonly ISystemAdminService _sysService;

        public SystemAdminController(ISystemAdminService sysService)
        {
            _sysService = sysService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var stats = await _sysService.GetSystemDashboardStatsAsync();
            return Ok(stats);
        }

        // Regions
        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions() => Ok(await _sysService.GetRegionsAsync());

        [HttpPost("regions")]
        public async Task<IActionResult> CreateRegion([FromBody] NameRequest req) => Ok(await _sysService.CreateRegionAsync(req.Name));

        [HttpPut("regions/{id}")]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateStatusRequest req) => Ok(await _sysService.UpdateRegionAsync(id, req.Name, req.Status));

        // Provinces
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces() => Ok(await _sysService.GetProvincesAsync());

        [HttpPost("provinces")]
        public async Task<IActionResult> CreateProvince([FromBody] CreateProvinceRequest req) => Ok(await _sysService.CreateProvinceAsync(req.Name, req.RegionId));

        [HttpPut("provinces/{id}")]
        public async Task<IActionResult> UpdateProvince(int id, [FromBody] UpdateProvinceRequest req) => Ok(await _sysService.UpdateProvinceAsync(id, req.Name, req.RegionId, req.Status));

        // PlaceTypes
        [HttpGet("placetypes")]
        public async Task<IActionResult> GetPlaceTypes() => Ok(await _sysService.GetPlaceTypesAsync());

        [HttpPost("placetypes")]
        public async Task<IActionResult> CreatePlaceType([FromBody] NameRequest req) => Ok(await _sysService.CreatePlaceTypeAsync(req.Name));

        [HttpPut("placetypes/{id}")]
        public async Task<IActionResult> UpdatePlaceType(int id, [FromBody] UpdateStatusRequest req) => Ok(await _sysService.UpdatePlaceTypeAsync(id, req.Name, req.Status));

        // Categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() => Ok(await _sysService.GetCategoriesAsync());

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest req) => Ok(await _sysService.CreateCategoryAsync(req.Name, req.PlaceTypeId));

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest req) => Ok(await _sysService.UpdateCategoryAsync(id, req.Name, req.PlaceTypeId, req.Status));

        // ReportReasons
        [HttpGet("reportreasons")]
        public async Task<IActionResult> GetReportReasons() => Ok(await _sysService.GetReportReasonsAsync());

        [HttpPost("reportreasons")]
        public async Task<IActionResult> CreateReportReason([FromBody] NameRequest req) => Ok(await _sysService.CreateReportReasonAsync(req.Name));

        [HttpPut("reportreasons/{id}")]
        public async Task<IActionResult> UpdateReportReason(int id, [FromBody] UpdateStatusRequest req) => Ok(await _sysService.UpdateReportReasonAsync(id, req.Name, req.Status));

        // Foods
        [HttpGet("foods")]
        public async Task<IActionResult> GetFoods() => Ok(await _sysService.GetFoodsAsync());

        [HttpPost("foods")]
        public async Task<IActionResult> CreateFood([FromBody] FoodRequest req) => Ok(await _sysService.CreateFoodAsync(req.Name, req.Description, req.ImageUrl));

        [HttpPut("foods/{id}")]
        public async Task<IActionResult> UpdateFood(long id, [FromBody] FoodRequest req) => Ok(await _sysService.UpdateFoodAsync(id, req.Name, req.Description, req.ImageUrl));

        [HttpDelete("foods/{id}")]
        public async Task<IActionResult> DeleteFood(long id) => Ok(await _sysService.DeleteFoodAsync(id));

        // FoodPlaces
        [HttpGet("foodplaces")]
        public async Task<IActionResult> GetFoodPlaces() => Ok(await _sysService.GetFoodPlacesAsync());

        [HttpPost("foodplaces")]
        public async Task<IActionResult> AddFoodPlace([FromBody] FoodPlaceRequest req) => Ok(await _sysService.AddFoodPlaceAsync(req.FoodId, req.PlaceId));

        [HttpDelete("foodplaces/{foodId}/{placeId}")]
        public async Task<IActionResult> RemoveFoodPlace(long foodId, long placeId) => Ok(await _sysService.RemoveFoodPlaceAsync(foodId, placeId));
    }

    public class NameRequest { public string Name { get; set; } = string.Empty; }
    public class UpdateStatusRequest { public string Name { get; set; } = string.Empty; public string Status { get; set; } = string.Empty; }
    
    public class CreateProvinceRequest { public string Name { get; set; } = string.Empty; public int RegionId { get; set; } }
    public class UpdateProvinceRequest { public string Name { get; set; } = string.Empty; public int RegionId { get; set; } public string Status { get; set; } = string.Empty; }
    
    public class CreateCategoryRequest { public string Name { get; set; } = string.Empty; public int PlaceTypeId { get; set; } }
    public class UpdateCategoryRequest { public string Name { get; set; } = string.Empty; public int PlaceTypeId { get; set; } public string Status { get; set; } = string.Empty; }
    
    public class FoodRequest { public string Name { get; set; } = string.Empty; public string? Description { get; set; } public string? ImageUrl { get; set; } }
    public class FoodPlaceRequest { public long FoodId { get; set; } public long PlaceId { get; set; } }
}
