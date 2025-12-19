using approval_workflow_backend.Guards;
using approval_workflow_backend.Infrastructure;
using approval_workflow_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace approval_workflow_backend.Services
{
    public class RequestService
    {
        private readonly AppDbContext _context;
        private string actorRole;

        public RequestService(AppDbContext context)
        {
            _context = context;
        }
        public void SubmitRequest(int requestId, int actorId, string actorRole)
        {//find the request of id given from requests
            var request = _context.Requests
                .IgnoreQueryFilters()
                .FirstOrDefault(r => r.Id == requestId);
            //if there is no such request or if its inactive, request not found
            if (request == null || !request.IsActive)
                throw new InvalidOperationException("Request Not Found");
            //if the current satate of the request cannot transition to Submitted, invalid state
            if (!RequestStateGuard.CanTransition(request.CurrentState, Models.RequestState.Submitted))
                throw new InvalidOperationException("Invalid state transition");
            //if fromState is the current state of request and toState is what it will be after trnsition
            var fromState = request.CurrentState;
            //change the current state to submitted and record the time.
            request.CurrentState = Models.RequestState.Submitted;
            request.SubmittedAt = DateTime.UtcNow;
            //update the request history in requestaudits
            _context.RequestAudits.Add(new Models.RequestAudit
            {
                RequestId = request.Id,
                ActorId = actorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.Submitted,
                Action = RequestAction.Submitted,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        public void AssignRequest(int requestId, int auditorId, int actorId, string actorRole)
        {//once request is submitted the request need to be assigned to an auditor and state must be transitioned.
            var request = _context.Requests.First(r => r.Id == requestId);
            //check if the currentstate can transition to assignedToAuditor
            if (!RequestStateGuard.CanTransition(
                request.CurrentState, RequestState.AssignedToAuditor))
                throw new InvalidOperationException("Invalid state transition");
            //check if the request has an assignement
            var activeAssignment = _context.RequestAssignments
                .FirstOrDefault(a => a.RequestId == requestId && a.IsActive);
            //if not assign one
            if (activeAssignment != null)
                activeAssignment.IsActive = false;
            _context.RequestAssignments.Add(new RequestAssignment
            {
                RequestId = requestId,
                AuditorId = auditorId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            });
            //change the state
            var fromState = request.CurrentState;
            request.CurrentState = RequestState.AssignedToAuditor;
            //Record in audit
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = actorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.AssignedToAuditor,
                Action = RequestAction.Assigned,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        public void MarkUnderReview(int requestId, int auditorId, string actorRole)
        {//once the request is opened by auditor its marked under auditor review
            var request = _context.Requests.First(r => r.Id == requestId);
            //check if its state can change from current to under auditor review
            if (!RequestStateGuard.CanTransition(
                request.CurrentState,
                RequestState.UnderAuditorReview))
                throw new InvalidOperationException("Invalid State Transfer");
            //get the assignment with the requestid, auditor id and is active.
            var assignment = _context.RequestAssignments
                .FirstOrDefault(a => a.RequestId == requestId &&
                a.AuditorId == auditorId && a.IsActive);
            //if null no such assignment
            if (assignment == null)
                throw new InvalidOperationException("No active assignments");
            //record the current state and change it to UnderAuditorView
            var fromState = request.CurrentState;
            request.CurrentState = RequestState.UnderAuditorReview;
            //record it in audits
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = auditorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.UnderAuditorReview,
                Action = RequestAction.Opened,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        //request is approved
        public void ApproveRequest(int requestId, int auditorId, string actorRole)
        {
            //get the request
            var request = _context.Requests.First(r => r.Id == requestId);
            //if the state cant change into approved, throwerror
            if (!RequestStateGuard.CanTransition(request.CurrentState, RequestState.Approved))
                throw new InvalidOperationException("Invalid State transition");
            //get assignment
            var assignment = _context.RequestAssignments.FirstOrDefault(r => r.RequestId == requestId && r.AuditorId == auditorId && r.IsActive == true);
            //if assignment null, then exception
            if (assignment == null)
                throw new InvalidOperationException("No active assignment for this request");
            //record state
            var fromState = request.CurrentState;
            //Change state
            request.CurrentState = RequestState.Approved;
            //close assignment
            assignment.IsActive = false;
            //record it under audit
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = auditorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.Approved,
                Action = RequestAction.Approved,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        //request is rejected
        public void RejectRequest(int requestId, int auditorId, string actorRole)
        {
            var request = _context.Requests.First(r => r.Id == requestId);
            if (!RequestStateGuard.CanTransition(request.CurrentState, RequestState.Rejected))
                throw new InvalidOperationException("Invalid State transition");
            var assignment = _context.RequestAssignments.FirstOrDefault(r => r.RequestId == requestId && r.AuditorId == auditorId && r.IsActive == true);
            if (assignment == null)
                throw new InvalidOperationException("No active assignment for the request");
            var fromState = request.CurrentState;
            request.CurrentState = RequestState.Rejected;
            assignment.IsActive = false;
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = auditorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.Rejected,
                Action = RequestAction.Rejected,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        //escalated to admin
        public void EscalateToAdmin(int requestId,int auditorId, string reason, string actorRole)
        {
            var request = _context.Requests.First(r => r.Id == requestId);
            if (!RequestStateGuard.CanTransition(
            request.CurrentState,
            RequestState.PendingAdmin))
                throw new InvalidOperationException("Invalid state transition");

            var assignment = _context.RequestAssignments.FirstOrDefault(r => r.RequestId == requestId && r.AuditorId == auditorId && r.IsActive);
            if (assignment == null) throw new InvalidOperationException("No active assignment for this request");
            var fromState = request.CurrentState;
            request.CurrentState = RequestState.PendingAdmin;
            assignment.IsActive = false;
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = auditorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.PendingAdmin,
                Action = RequestAction.Escalated,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
            
            
        }
        //close request
        public void CloseRequest(int requestId,int actorId, string actorRole)
        {
            var request = _context.Requests.First(r => r.Id == requestId);
            if (!RequestStateGuard.CanTransition(request.CurrentState, RequestState.Closed))
                throw new InvalidOperationException("Invalid state transition");
            var fromState = request.CurrentState;
            request.CurrentState = RequestState.Closed;
            request.ClosedAt = DateTime.UtcNow;
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = actorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.Closed,
                Action = RequestAction.Closed,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        //Deactivete request
        public void DeactivateRequest(int requestId, int actorId,string reason, string role)
        {
            var request = _context.Requests.IgnoreQueryFilters().First(r => r.Id == requestId);
            if (request.CurrentState != RequestState.Closed) throw new InvalidOperationException("Only closed requests can be deactivated");
            if (!request.IsActive) return;
            request.IsActive = false;
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = actorId,
                ActorRole = actorRole,
                FromState = request.CurrentState,
                ToState = request.CurrentState,
                Action = RequestAction.Deactivated,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        //redressalls
        public void CreateRedressal(int requestId, int actorId, string payload, string actorRole)
        {
            var request = _context.Requests.IgnoreQueryFilters().First(r=> r.Id == requestId);
            if (request.CurrentState != RequestState.Closed) throw new InvalidOperationException("Only closed requests can have redressals");
            request.RedressalCount++;
            var redressal = new Redressal
            {
                RequestId = requestId,
                RedressalCount = request.RedressalCount,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Redressals.Add(redressal);
            _context.RedressalContents.Add(new RedressalContent
            {
                Redressal = redressal,
                PayLoad = payload,
                CreatedAt = DateTime.UtcNow
            });
            var fromState = request.CurrentState;
            request.CurrentState = RequestState.Submitted;
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = actorId,
                ActorRole = actorRole,
                FromState = fromState,
                ToState = RequestState.Submitted,
                Action = RequestAction.Created,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
        public void CloseRedressal(int requestId,int actorId,string reason, string actorRole)
        {
            var request = _context.Requests.First(r => r.Id == requestId);
            if (request.CurrentState!=RequestState.Closed)
                throw new InvalidOperationException("Redressal can only be closed after the request is closed");
            var redressal = _context.Redressals.FirstOrDefault(r => r.RequestId == requestId && r.IsActive);
            if (redressal == null) throw new InvalidOperationException("No active reddressal exists for this request");
            redressal.IsActive = false;
            redressal.ClosedAt = DateTime.UtcNow;
            _context.RequestAudits.Add(new RequestAudit
            {
                RequestId = requestId,
                ActorId = actorId,
                ActorRole = actorRole,
                FromState = RequestState.Closed,
                ToState = RequestState.Closed,
                Reason = reason,
                Action = RequestAction.RedressalClosed,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
    }
}
