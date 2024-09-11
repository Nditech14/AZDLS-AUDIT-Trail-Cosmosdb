using Cosmos.Application.Services;
using Cosmos.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosmos.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        // Get all audit logs
        [HttpGet("GetAllAuditLogs")]
        public async Task<ActionResult<IEnumerable<Audit>>> GetAllAuditLogs()
        {
            var audits = await _auditService.GetAllAuditLogsAsync();
            if (audits == null || !audits.Any())
            {
                return NotFound("No audit logs found.");
            }
            return Ok(audits);
        }

        // Get audit logs by user ID
        [HttpGet("GetAuditLogsByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Audit>>> GetAuditLogsByUser(string userId)
        {
            var audits = await _auditService.GetAuditLogsByUserAsync(userId);
            if (audits == null || !audits.Any())
            {
                return NotFound($"No audit logs found for user ID: {userId}");
            }
            return Ok(audits);
        }

        //// Get audit logs by action type
        //[HttpGet("GetAuditLogsByAction/{action}")]
        //public async Task<ActionResult<IEnumerable<Audit>>> GetAuditLogsByAction(string action)
        //{
        //    var audits = await _auditService.GetAuditLogsByActionAsync(action);
        //    if (audits == null)
        //    {
        //        return NotFound($"No audit logs found for action: {action}");
        //    }
        //    return Ok(audits);
        //}

        // Get audit logs by date range
        [HttpGet("GetAuditLogsByDateRange")]
        public async Task<ActionResult<IEnumerable<Audit>>> GetAuditLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var audits = await _auditService.GetAuditLogsByDateRangeAsync(startDate, endDate);
            if (audits == null || !audits.Any())
            {
                return NotFound("No audit logs found within the specified date range.");
            }
            return Ok(audits);
        }
    }
}
