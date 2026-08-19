using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateReview([FromBody] AddReviewDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized(new { message = "Vui lòng đăng nhập để đánh giá." });
            try
            {
                var review = await _reviewService.AddOrUpdateReviewAsync(userId, dto);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(long id, [FromQuery] long userId, [FromQuery] bool isAdmin = false)
        {
            if (userId <= 0) return Unauthorized();
            var success = await _reviewService.DeleteReviewAsync(userId, id, isAdmin);
            return success ? Ok(new { message = "Đã xóa bài đánh giá." }) : BadRequest(new { message = "Không thể xóa bài đánh giá." });
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized(new { message = "Vui lòng đăng nhập để bình luận." });
            try
            {
                var comment = await _reviewService.AddCommentAsync(userId, dto);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(long id, [FromQuery] long userId, [FromQuery] bool isAdmin = false)
        {
            if (userId <= 0) return Unauthorized();
            var success = await _reviewService.DeleteCommentAsync(userId, id, isAdmin);
            return success ? Ok(new { message = "Đã xóa bình luận." }) : BadRequest();
        }

        [HttpPost("reports")]
        public async Task<IActionResult> Report([FromBody] CreateReportDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized(new { message = "Vui lòng đăng nhập để báo cáo nội dung." });
            try
            {
                var success = await _reviewService.ReportTargetAsync(userId, dto);
                return Ok(new { message = "Báo cáo của bạn đã được gửi tới Ban quản trị để kiểm duyệt." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("appeals")]
        public async Task<IActionResult> Appeal([FromBody] CreateAppealDto dto, [FromQuery] long userId)
        {
            if (userId <= 0) return Unauthorized(new { message = "Vui lòng đăng nhập để khiếu nại." });
            try
            {
                var success = await _reviewService.SubmitAppealAsync(userId, dto);
                return Ok(new { message = "Khiếu nại của bạn đã được tiếp nhận." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
