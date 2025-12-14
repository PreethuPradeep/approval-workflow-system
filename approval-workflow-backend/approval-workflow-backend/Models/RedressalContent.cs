using ApprovalWorkflow.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class RedressalContent
    {
        [Key]
        public int RedressalId { get; set; }
        public Redressal Redressal { get; set; }
        [Required]
        public string PayLoad { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
