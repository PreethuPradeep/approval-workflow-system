using System.ComponentModel.DataAnnotations;

namespace approval_workflow_backend.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
