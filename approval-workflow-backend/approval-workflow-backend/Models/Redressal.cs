using approval_workflow_backend.Models;
using System.ComponentModel.DataAnnotations;

namespace ApprovalWorkflow.Domain.Models
{
    public class Redressal
    {
        public int Id { get; set; }
        [Required]
        public int RequestId { get; set; }
        public Request Request { get; set; }
        [Required]
        public int RedressalCount { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

    }
}