using ApprovalWorkflow.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class RequestContent
    {
        [Key]
        public int RequestId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Request Request { get; set; }
    }
}
