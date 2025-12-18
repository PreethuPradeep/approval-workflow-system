
using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        // Who created this request
        [Required]
        public int RequesterId { get; set; }
        public User Requester { get; set; }

        // State machine anchor
        [Required]
        public RequestState CurrentState { get; set; }

        // Redressal tracking
        [Required]
        public int RedressalCount { get; set; } = 0;

        // Soft delete / visibility control
        //When false, the request is considered archived (soft-deleted) and is
        /// excluded by global query filters. Historical child records may still exist.
        public bool IsActive { get; set; } = true;

        // Timestamps for lifecycle insight
        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Navigation
        public RequestContent Content { get; set; }
        public ICollection<Redressal> Redressals { get; set; }
        public ICollection<RequestAudit> Audits { get; set; }
        public ICollection<RequestAssignment> Assignments { get; set; }
    }
}
