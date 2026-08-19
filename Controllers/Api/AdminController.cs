using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats([FromQuery] long adminUserId, [FromQuery] bool isSystemAdmin = false)
        {
            var stats = await _adminService.GetAdminDashboardStatsAsync(adminUserId, isSystemAdmin);
            return Ok(stats);
        }

        // Pending places
        [HttpGet("pending-places")]
        public async Task<IActionResult> GetPendingPlaces([FromQuery] long adminUserId, [FromQuery] bool isSystemAdmin = false)
        {
            var list = await _adminService.GetPendingPlacesForAdminAsync(adminUserId, isSystemAdmin);
            return Ok(list);
        }

        [HttpPost("places/{id}/approve")]
        public async Task<IActionResult> ApprovePlace(long id, [FromQuery] long adminUserId)
        {
            var success = await _adminService.ApprovePlaceAsync(id, adminUserId);
            return success ? Ok(new { message = "Đã duyệt địa điểm thành công." }) : BadRequest();
        }

        [HttpPost("places/{id}/reject")]
        public async Task<IActionResult> RejectPlace(long id, [FromBody] RejectRequest req, [FromQuery] long adminUserId)
        {
            var success = await _adminService.RejectPlaceAsync(id, adminUserId, req.Reason);
            return success ? Ok(new { message = "Đã từ chối địa điểm." }) : BadRequest();
        }

        // Edit proposals
        [HttpGet("pending-edits")]
        public async Task<IActionResult> GetPendingEdits([FromQuery] long adminUserId, [FromQuery] bool isSystemAdmin = false)
        {
            var list = await _adminService.GetPendingEditProposalsAsync(adminUserId, isSystemAdmin);
            return Ok(list);
        }

        [HttpPost("edits/{id}/approve")]
        public async Task<IActionResult> ApproveEdit(long id, [FromQuery] long adminUserId)
        {
            var success = await _adminService.ApproveEditProposalAsync(id, adminUserId);
            return success ? Ok(new { message = "Đã chấp nhận và cập nhật thông tin địa điểm." }) : BadRequest();
        }

        [HttpPost("edits/{id}/reject")]
        public async Task<IActionResult> RejectEdit(long id, [FromBody] RejectRequest req, [FromQuery] long adminUserId)
        {
            var success = await _adminService.RejectEditProposalAsync(id, adminUserId, req.Reason);
            return success ? Ok(new { message = "Đã từ chối đề xuất chỉnh sửa." }) : BadRequest();
        }

        // Reports
        [HttpGet("reports")]
        public async Task<IActionResult> GetReports([FromQuery] long adminUserId, [FromQuery] bool isSystemAdmin = false)
        {
            var list = await _adminService.GetReportsForAdminAsync(adminUserId, isSystemAdmin);
            return Ok(list);
        }

        [HttpPost("reports/{id}/resolve")]
        public async Task<IActionResult> ResolveReport(long id, [FromBody] ResolveReportRequest req, [FromQuery] long adminUserId)
        {
            var success = await _adminService.ResolveReportAsync(id, adminUserId, req.ConfirmViolation, req.Note);
            return success ? Ok(new { message = "Đã xử lý báo cáo thành công." }) : BadRequest();
        }

        // Appeals
        [HttpGet("appeals")]
        public async Task<IActionResult> GetAppeals([FromQuery] long adminUserId, [FromQuery] bool isSystemAdmin = false)
        {
            var list = await _adminService.GetAppealsAsync(adminUserId, isSystemAdmin);
            return Ok(list);
        }

        [HttpPost("appeals/{id}/handle")]
        public async Task<IActionResult> HandleAppeal(long id, [FromBody] HandleAppealRequest req, [FromQuery] long adminUserId, [FromQuery] bool isSystemAdmin = false)
        {
            var success = await _adminService.HandleAppealAsync(id, adminUserId, isSystemAdmin, req.Result, req.Escalate);
            return success ? Ok(new { message = "Đã xử lý khiếu nại thành công." }) : BadRequest();
        }

        // System Admin: Users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _adminService.GetAllUsersAsync();
            return Ok(list);
        }

        [HttpPost("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(long id, [FromQuery] long adminUserId)
        {
            var success = await _adminService.ToggleUserStatusAsync(id, adminUserId);
            return success ? Ok(new { message = "Cập nhật trạng thái người dùng thành công." }) : BadRequest();
        }

        [HttpPost("users/{id}/role")]
        public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleRequest req, [FromQuery] long adminUserId)
        {
            var success = await _adminService.UpdateUserRoleAsync(id, req.Role, adminUserId);
            return success ? Ok(new { message = "Cập nhật vai trò thành công." }) : BadRequest();
        }

        [HttpPost("users/{id}/assign-categories")]
        public async Task<IActionResult> AssignCategories(long id, [FromBody] AssignCategoryRequest req, [FromQuery] long adminUserId)
        {
            var success = await _adminService.AssignCategoriesToAdminAsync(id, req.CategoryIds, adminUserId);
            return success ? Ok(new { message = "Phân quyền danh mục thành công." }) : BadRequest();
        }

        // System Configs
        [HttpGet("configs")]
        public async Task<IActionResult> GetConfigs()
        {
            var list = await _adminService.GetSystemConfigsAsync();
            return Ok(list);
        }

        [HttpPost("configs")]
        public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfigRequest req, [FromQuery] long adminUserId)
        {
            var success = await _adminService.UpdateSystemConfigAsync(req.Key, req.Value, adminUserId);
            return success ? Ok(new { message = "Cập nhật cấu hình thành công." }) : BadRequest();
        }

        // Audit Logs
        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int limit = 50)
        {
            var list = await _adminService.GetAuditLogsAsync(limit);
            return Ok(list);
        }
    }

    public class RejectRequest { public string Reason { get; set; } = string.Empty; }
    public class ResolveReportRequest { public bool ConfirmViolation { get; set; } public string? Note { get; set; } }
    public class HandleAppealRequest { public string Result { get; set; } = string.Empty; public bool Escalate { get; set; } }
    public class UpdateRoleRequest { public string Role { get; set; } = "user"; }
    public class AssignCategoryRequest { public List<int> CategoryIds { get; set; } = new(); }
    public class UpdateConfigRequest { public string Key { get; set; } = string.Empty; public string Value { get; set; } = string.Empty; }
}
