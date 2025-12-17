using approval_workflow_backend.Guards;
using approval_workflow_backend.Infrastructure;
using approval_workflow_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace approval_workflow_backend.Services
{
    public class RequestService
    {
        private readonly AppDbContext _context;
        public RequestService(AppDbContext context)
        {
            _context = context;
        }
        public void SubmitRequest(int requestId, int actorId)
        {
            var request = _context.Requests
                .IgnoreQueryFilters()
                .FirstOrDefault(r => r.Id == requestId);
            if (request == null || !request.IsActive)
                throw new InvalidOperationException("Request Not Found");
            if (!RequestStateGuard.CanTransition(request.CurrentState, Models.RequestState.Submitted))
                throw new InvalidOperationException("Invalid state transition");

            var fromState = request.CurrentState;
            request.CurrentState = Models.RequestState.Submitted;
            request.SubmittedAt = DateTime.UtcNow;

            _context.RequestAudits.Add(new Models.RequestAudit
            {
                RequestId = request.Id,
                ActorId = actorId,
                FromState = fromState,
                ToState = RequestState.Submitted,
                Action = RequestAction.Submitted,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }

    }
}
