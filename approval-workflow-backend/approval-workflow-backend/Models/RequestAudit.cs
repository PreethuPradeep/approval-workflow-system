

using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class RequestAudit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RequestId { get; set; }
        public Request MyProperty { get; set; }
        
        public int? RedressalId { get; set; }
        public Redressal Redressal { get; set; }
        [Required]
        public int ActorId { get; set; }
        public User Actor { get; set; }
        [Required]
        public string ActorRole { get; set; }
        [Required]
        public RequestState FromState { get; set; }
        [Required]
        public RequestState ToState { get; set; }
        [Required]
        public RequestAction Action { get; set; }
        public string Reason { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

    }
}