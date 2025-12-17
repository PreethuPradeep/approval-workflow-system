using approval_workflow_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace approval_workflow_backend.Infrastructure
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestContent> RequestContents { get; set; }
        public DbSet<RequestAssignment> RequestAssignments { get; set; }
        public DbSet<Redressal> Redressals { get; set; }
        public DbSet<RedressalContent> RedressalContents { get; set; }
        public DbSet<RequestAudit> RequestAudits { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Declares a composite primary key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            //one user has many useroles and enforces fk
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            //role to userrole relationship
            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(r => r.RoleId);
            //request to requestcontent, one to one, fk
            modelBuilder.Entity<Request>()
                .HasOne(rc => rc.Content)
                .WithOne(c => c.Request)
                .HasForeignKey<RequestContent>(c => c.RequestId);
            //Request ↔ RequestAssignment (one-to-many) fk
            modelBuilder.Entity<RequestAssignment>()
                .HasOne(r => r.Request)
                .WithMany(rq => rq.Assignments)
                .HasForeignKey(r => r.RequestId);
            //only one active assignment
            modelBuilder.Entity<RequestAssignment>()
                .HasIndex(a => new { a.RequestId, a.IsActive })
                .IsUnique();
            //redressal count is unique per request thats active
            modelBuilder.Entity<Redressal>()
                .HasIndex(r => new { r.RequestId, r.RedressalCount, r.IsActive })
                .IsUnique();
            modelBuilder.Entity<Request>()
                .HasQueryFilter(r => r.IsActive);
            modelBuilder.Entity<Request>()
                .Property(r => r.CurrentState)
                .HasConversion<int>();
            //child should survive if parent is soft deleted
            modelBuilder.Entity<RequestAudit>()
                .HasOne(a => a.Request)
                .WithMany(r => r.Audits)
                .HasForeignKey(a => a.RequestId)
                .IsRequired(false);
            modelBuilder.Entity<RequestAssignment>()
                .HasOne(ass => ass.Request)
                .WithMany(r => r.Assignments)
                .HasForeignKey(ass => ass.RequestId)
                .IsRequired(false);
            modelBuilder.Entity<Redressal>()
                .HasOne(rd => rd.Request)
                .WithMany(r => r.Redressals)
                .HasForeignKey(rd => rd.RequestId)
                .IsRequired(false);
            modelBuilder.Entity<RequestContent>()
                .HasOne(rc => rc.Request)
                .WithOne(r => r.Content)
                .HasForeignKey<RequestContent>(rc => rc.RequestId)
                .IsRequired(false);
        }
    }
}
