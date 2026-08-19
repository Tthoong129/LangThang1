using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMap.Data;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly TravelReviewDbContext _db;

        public MetadataController(TravelReviewDbContext db)
        {
            _db = db;
        }

        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions()
        {
            var regions = await _db.Regions
                .Where(r => r.Status == "active")
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();
            return Ok(regions);
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces([FromQuery] int? regionId)
        {
            var query = _db.Provinces.Where(p => p.Status == "active");
            if (regionId.HasValue && regionId.Value > 0)
            {
                query = query.Where(p => p.RegionId == regionId.Value);
            }
            var provinces = await query
                .Select(p => new { p.Id, p.Name, p.RegionId, RegionName = p.Region!.Name })
                .ToListAsync();
            return Ok(provinces);
        }

        [HttpGet("place-types")]
        public async Task<IActionResult> GetPlaceTypes()
        {
            var types = await _db.PlaceTypes
                .Where(t => t.Status == "active")
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();
            return Ok(types);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int? placeTypeId)
        {
            var query = _db.Categories.Where(c => c.Status == "active");
            if (placeTypeId.HasValue && placeTypeId.Value > 0)
            {
                query = query.Where(c => c.PlaceTypeId == placeTypeId.Value);
            }
            var categories = await query
                .Select(c => new { c.Id, c.Name, c.PlaceTypeId, PlaceTypeName = c.PlaceType!.Name })
                .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("report-reasons")]
        public async Task<IActionResult> GetReportReasons()
        {
            var reasons = await _db.ReportReasons
                .Where(r => r.Status == "active")
                .Select(r => new { r.Id, r.Content })
                .ToListAsync();
            return Ok(reasons);
        }
    }
}
