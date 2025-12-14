namespace approval_workflow_backend.Models
{
    public enum RequestState
    {
        Draft = 1,//request created
        Submitted = 2,//request finalised and now system will assign a auditor and queue it
        AssignedToAuditor = 3,//auditor assigned
        UnderAuditorReview = 4,//auditor opened the request
        PendingAdmin =5,//request escalated to admin
        Approved = 6,
        Rejexted = 7,
        Closed = 8//request lifecycle is over
    }
}
