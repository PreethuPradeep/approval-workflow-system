using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        public string PsswordHash { get; set; }

        public bool Active { get; set; } = true;
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
