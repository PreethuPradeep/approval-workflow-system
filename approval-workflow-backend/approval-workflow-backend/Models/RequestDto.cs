namespace approval_workflow_backend.Models
{
    public class AssignRequestDto
    {
        public int AuditorId { get; set; }
    }

    public class ReasonDto
    {
        public string Reason { get; set; }
    }

    public class CreateRedressalDto
    {
        public string Payload { get; set; }
    }

}
