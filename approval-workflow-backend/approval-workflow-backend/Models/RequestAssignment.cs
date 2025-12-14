using ApprovalWorkflow.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class RequestAssignment
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        [Required]
        public Request Request { get; set; }
        [Required]
        public int AuditorId { get; set; }
        public User Auditor { get; set; }
        [Required]
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
