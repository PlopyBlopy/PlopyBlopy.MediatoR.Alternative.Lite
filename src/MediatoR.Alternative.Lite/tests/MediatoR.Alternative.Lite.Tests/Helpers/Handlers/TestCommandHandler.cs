using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;

namespace MediatoR.Alternative.Lite.Tests.Helpers.Handlers
{
    public class TestCommandHandler : ICommandHandler<TestCommand, TestResponse>
    {
        public Task<Result<TestResponse>> Handle(TestCommand request, CancellationToken ct)
        {
            return Task.FromResult(Result.Ok(new TestResponse()));
        }
    }
}