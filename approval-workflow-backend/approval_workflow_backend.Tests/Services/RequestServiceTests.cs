using approval_workflow_backend.Models;
using approval_workflow_backend.Services;
using approval_workflow_backend.Infrastructure;
using approval_workflow_backend.Utility;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class RequestServiceTests
{
	[Fact]
    public void SubmitRequest_Moves_draft_To_Submitted_And_Adds_Audit()
    {
        using var context = CreateContext();

        SeedUserWithRole(context, userId: 1, roleName: Roles.Requestor);

        var request = new Request
        {
            RequesterId = 1,
            CurrentState = RequestState.Draft,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Requests.Add(request);
        context.SaveChanges();

        var service = new RequestService(context);

        service.SubmitRequest(request.Id, actorId: 1);

        var updated = context.Requests.First();
        Assert.Equal(RequestState.Submitted, updated.CurrentState);
        Assert.NotNull(updated.SubmittedAt);
        Assert.Single(context.RequestAudits);
    }

    [Fact]
	public void ApproveRequest_From_Draft_Throws()
	{
		using var context = CreateContext();
		var request = new Request
		{
			RequesterId = 1,
			CurrentState = RequestState.Draft,
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};
        context.Requests.Add(request);
        context.SaveChanges();

        var service = new RequestService(context);

        Assert.Throws<InvalidOperationException>(() =>
            service.ApproveRequest(
				request.Id,
				auditorId: 2
				//,actorRole: "Auditor"
			));

    }
    [Fact]
    public void AssignRequest_From_Draft_Throws()
    {
        using var context = CreateContext();
        SeedUserWithRole(context, userId: 1, roleName: Roles.Requestor);


        var request = new Request
        {
            RequesterId = 1,
            CurrentState = RequestState.Draft,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Requests.Add(request);
        context.SaveChanges();

        var service = new RequestService(context);

        Assert.Throws<InvalidOperationException>(() =>
            service.AssignRequest(request.Id, auditorId: 2, actorId: 99));
    }
    [Fact]
    public void AssignRequest_Creates_Active_Assignment()
    {
        using var context = CreateContext();
        SeedUserWithRole(context, userId: 99, roleName: Roles.Admin);
        SeedUserWithRole(context, userId: 2, roleName: Roles.Auditor);

        var request = new Request
        {
            RequesterId = 1,
            CurrentState = RequestState.Submitted,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Requests.Add(request);
        context.SaveChanges();

        var service = new RequestService(context);
        service.AssignRequest(request.Id, auditorId: 2, actorId: 99);

        var assignment = context.RequestAssignments.Single();

        Assert.True(assignment.IsActive);
        Assert.Equal(2, assignment.AuditorId);
    }
    [Fact]
    public void ApproveRequest_By_NonAssigned_Auditor_Throws()
    {
        using var context = CreateContext();
        SeedUserWithRole(context, userId: 1, roleName: "Auditor");
        SeedUserWithRole(context, userId: 2, roleName: "Auditor");
            var request = new Request
            {
                RequesterId = 5,
                CurrentState = RequestState.UnderAuditorReview,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        context.Requests.Add(request);
        context.SaveChanges();
        context.RequestAssignments.Add(new RequestAssignment
        {
            RequestId = request.Id,
            AuditorId = 1,
            IsActive = true,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();
        var service = new RequestService(context);
        Assert.Throws<InvalidOperationException>(()=>
        service.ApproveRequest(request.Id, auditorId: 2));
    }

    private static AppDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new AppDbContext(options);
	}
    private static void SeedUserWithRole(
    AppDbContext context,
    int userId,
    string roleName)
    {
        var role = new Role { Name = roleName };

        var user = new User
        {
            Id = userId,
            Email = $"user{userId}@test.com",
            FullName = $"Test User {userId}",
            PasswordHash = "hashed-password"
        };

        context.Roles.Add(role);
        context.Users.Add(user);

        context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            Role = role
        });

        context.SaveChanges();
    }


}
