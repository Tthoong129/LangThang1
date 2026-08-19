using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userService.AuthenticateAsync(req.Email, req.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa." });
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto req)
        {
            try
            {
                var user = await _userService.RegisterAsync(req);
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(long id)
        {
            var user = await _userService.GetProfileAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(long id, [FromBody] UpdateProfileDto dto)
        {
            var success = await _userService.UpdateProfileAsync(id, dto);
            return success ? Ok(new { message = "Cập nhật thông tin thành công." }) : BadRequest();
        }

        [HttpPost("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordRequest req)
        {
            var success = await _userService.ChangePasswordAsync(id, req.OldPassword, req.NewPassword);
            return success ? Ok(new { message = "Đổi mật khẩu thành công." }) : BadRequest(new { message = "Mật khẩu cũ không chính xác." });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
