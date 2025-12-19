using approval_workflow_backend.Models;
using approval_workflow_backend.Services;
using approval_workflow_backend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class RequestServiceTests
{
	[Fact]
	public void SubmitRequest_Moves_draft_To_Submitted_And_Adds_Audit()
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
            service.ApproveRequest(request.Id, auditorId: 2));
    }
	private static AppDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new AppDbContext(options);
	}
}
