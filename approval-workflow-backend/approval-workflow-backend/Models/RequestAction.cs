namespace approval_workflow_backend.Models
{
    public enum RequestAction
    {
        Created = 1,// Request initially created (Draft)
        Submitted = 2,// Request submitted by requester
        Assigned = 3,// Assigned to an auditor
        Opened = 4,// Auditor opened / started reviewing
        Escalated = 5,// Sent to admin
        Approved = 6,// Approved by auditor/admin
        Rejected = 7,// Rejected by auditor/admin
        Closed = 8,// Closed after approval/rejection
        Deactivated = 9// Soft-deleted / archived
    }
}
