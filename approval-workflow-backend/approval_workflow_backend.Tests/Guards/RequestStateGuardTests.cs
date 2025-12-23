using approval_workflow_backend.Guards;
using approval_workflow_backend.Models;
using Xunit;

public class RequestStateGuardTests
{
    [Theory]
    [InlineData(RequestState.Draft, RequestState.Submitted)]
    [InlineData(RequestState.Submitted, RequestState.AssignedToAuditor)]
    [InlineData(RequestState.AssignedToAuditor, RequestState.UnderAuditorReview)]
    [InlineData(RequestState.UnderAuditorReview, RequestState.Approved)]
    [InlineData(RequestState.UnderAuditorReview, RequestState.Rejected)]
    public void Allows_valid_transitions(RequestState from, RequestState to)
    {
        Assert.True(RequestStateGuard.CanTransition(from, to));
    }

    [Theory]
    [InlineData(RequestState.Draft, RequestState.Approved)]
    [InlineData(RequestState.Closed, RequestState.Submitted)]
    [InlineData(RequestState.Rejected, RequestState.AssignedToAuditor)]
    public void Blocks_invalid_transitions(RequestState from, RequestState to)
    {
        Assert.False(RequestStateGuard.CanTransition(from, to));
    }
}
