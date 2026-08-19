using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlacesController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] PlaceFilterDto filter)
        {
            var places = await _placeService.SearchPlacesAsync(filter);
            return Ok(places);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(long id, [FromQuery] long? currentUserId = null)
        {
            var place = await _placeService.GetPlaceDetailAsync(id, currentUserId);
            if (place == null) return NotFound(new { message = "Địa điểm không tồn tại." });
            return Ok(place);
        }

        [HttpPost("{id}/view")]
        public async Task<IActionResult> TrackView(long id, [FromQuery] long userId)
        {
            // Log access history silently
            if (userId > 0) {
                await _placeService.RecordAccessHistoryAsync(id, userId);
            }
            return Ok();
        }

        [HttpPost("propose")]
        public async Task<IActionResult> ProposePlace([FromBody] ProposePlaceDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized(new { message = "Vui lòng đăng nhập để đề xuất địa điểm." });
            var place = await _placeService.ProposePlaceAsync(dto, userId);
            return Ok(new { message = "Đã gửi đề xuất địa điểm thành công, vui lòng chờ kiểm duyệt.", placeId = place.Id });
        }

        [HttpPost("propose-edit")]
        public async Task<IActionResult> ProposeEdit([FromBody] ProposeEditDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized(new { message = "Vui lòng đăng nhập để đề xuất chỉnh sửa." });
            var proposal = await _placeService.ProposeEditAsync(dto, userId);
            return Ok(new { message = "Đã gửi đề xuất chỉnh sửa thành công, vui lòng chờ kiểm duyệt.", proposalId = proposal.Id });
        }

        [HttpGet("rankings")]
        public async Task<IActionResult> GetRankings([FromQuery] int? provinceId, [FromQuery] int? regionId, [FromQuery] int? placeTypeId, [FromQuery] int limit = 10)
        {
            var rankings = await _placeService.GetTopRankedPlacesAsync(provinceId, regionId, placeTypeId, limit);
            return Ok(rankings);
        }
    }
}
