using approval_workflow_backend.Models;
using approval_workflow_backend.Services;
using approval_workflow_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace approval_workflow_backend.Controllers
{
    [Route("api/requests")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly RequestService _requestService;

        public RequestsController(RequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost("{id}/submit")]
        [Authorize(Roles = Roles.Requestor)]
        public IActionResult Submit(int id)
        {
            var actorId = GetUserId();
            _requestService.SubmitRequest(id, actorId);
            return NoContent();
        }

        [HttpPost("{id}/assign")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Assign(int id, [FromBody] AssignRequestDto dto)
        {
            var actorId = GetUserId();
            _requestService.AssignRequest(id, dto.AuditorId, actorId);
            return NoContent();
        }

        [HttpPost("{id}/open")]
        [Authorize(Roles = Roles.Auditor)]
        public IActionResult Open(int id)
        {
            var auditorId = GetUserId();
            _requestService.MarkUnderReview(id, auditorId);
            return NoContent();
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = Roles.Auditor)]
        public IActionResult Approve(int id)
        {
            var auditorId = GetUserId();
            _requestService.ApproveRequest(id, auditorId);
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Auditor)]
        public IActionResult Reject(int id)
        {
            var auditorId = GetUserId();
            _requestService.RejectRequest(id, auditorId);
            return NoContent();
        }

        [HttpPost("{id}/escalate")]
        [Authorize(Roles = Roles.Auditor)]
        public IActionResult Escalate(int id, [FromBody] ReasonDto dto)
        {
            var auditorId = GetUserId();
            _requestService.EscalateToAdmin(id, auditorId, dto.Reason);
            return NoContent();
        }

        [HttpPost("{id}/close")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Close(int id)
        {
            var actorId = GetUserId();
            _requestService.CloseRequest(id, actorId);
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Deactivate(int id, [FromBody] ReasonDto dto)
        {
            var actorId = GetUserId();
            _requestService.DeactivateRequest(id, actorId, dto.Reason);
            return NoContent();
        }

        [HttpPost("{id}/redressals")]
        [Authorize(Roles = Roles.Requestor)]
        public IActionResult CreateRedressal(int id, [FromBody] CreateRedressalDto dto)
        {
            var actorId = GetUserId();
            _requestService.CreateRedressal(id, actorId, dto.Payload);
            return NoContent();
        }

        [HttpPost("{id}/redressals/close")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult CloseRedressal(int id, [FromBody] ReasonDto dto)
        {
            var actorId = GetUserId();
            _requestService.CloseRedressal(id, actorId, dto.Reason);
            return NoContent();
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("User identifier claim missing");

            return int.Parse(claim.Value);
        }
    }

}
