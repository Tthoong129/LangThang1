using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Profile
        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetPublicProfile(long id)
        {
            var profile = await _userService.GetPublicProfileAsync(id);
            if (profile == null) return NotFound("Người dùng không tồn tại.");
            return Ok(profile);
        }

        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetUserReviews(long id)
        {
            var reviews = await _userService.GetUserReviewsAsync(id);
            return Ok(reviews);
        }

        // Favorites
        [HttpPost("favorites/{placeId}")]
        public async Task<IActionResult> ToggleFavorite(long placeId, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var added = await _userService.ToggleFavoriteAsync(userId, placeId);
            return Ok(new { isFavorite = added, message = added ? "Đã thêm vào danh sách yêu thích" : "Đã bỏ khỏi danh sách yêu thích" });
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites([FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var list = await _userService.GetFavoritesAsync(userId);
            return Ok(list);
        }

        // Visit Logs
        [HttpPost("visit-logs")]
        public async Task<IActionResult> AddVisitLog([FromBody] AddVisitLogDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var log = await _userService.AddVisitLogAsync(userId, dto);
            return Ok(log);
        }

        [HttpDelete("visit-logs/{id}")]
        public async Task<IActionResult> RemoveVisitLog(long id, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var success = await _userService.RemoveVisitLogAsync(userId, id);
            return success ? Ok(new { message = "Đã xóa nhật ký ghé thăm." }) : BadRequest();
        }

        [HttpGet("visit-logs")]
        public async Task<IActionResult> GetVisitLogs([FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var list = await _userService.GetUserVisitLogsAsync(userId, true);
            return Ok(list);
        }

        // Access History
        [HttpGet("access-history")]
        public async Task<IActionResult> GetAccessHistory([FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var list = await _userService.GetAccessHistoryAsync(userId);
            return Ok(list);
        }

        [HttpDelete("access-history/{id}")]
        public async Task<IActionResult> DeleteAccessHistoryItem(long id, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var success = await _userService.RemoveAccessHistoryItemAsync(userId, id);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("access-history")]
        public async Task<IActionResult> ClearAccessHistory([FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            await _userService.ClearAccessHistoryAsync(userId);
            return Ok(new { message = "Đã xóa toàn bộ lịch sử truy cập." });
        }

        // Notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications([FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            var list = await _userService.GetNotificationsAsync(userId);
            return Ok(list);
        }

        [HttpPost("notifications/{id}/read")]
        public async Task<IActionResult> MarkNotificationRead(long id, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized();
            await _userService.MarkNotificationReadAsync(userId, id);
            return Ok();
        }
    }
}
