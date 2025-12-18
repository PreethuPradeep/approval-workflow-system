using approval_workflow_backend.Models;

namespace approval_workflow_backend.Guards
{
    public class RequestStateGuard
    {
        public static bool CanTransition(RequestState from,RequestState to)
        {
            return (from, to) switch
            {
                (RequestState.Draft,RequestState.Submitted) => true,
                (RequestState.Submitted, RequestState.AssignedToAuditor) => true,
                (RequestState.AssignedToAuditor, RequestState.UnderAuditorReview) => true,
                (RequestState.UnderAuditorReview, RequestState.Approved) => true,
                (RequestState.UnderAuditorReview,RequestState.Rejected) => true,
                (RequestState.UnderAuditorReview, RequestState.PendingAdmin) =>true,
                (RequestState.Approved,RequestState.Closed)=>true,
                (RequestState.Rejected,RequestState.Closed)=>true,
                _=>false
            };
        }
    }
}
