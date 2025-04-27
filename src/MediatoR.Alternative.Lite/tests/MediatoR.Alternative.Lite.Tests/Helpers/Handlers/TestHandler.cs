using FluentResults;

namespace MediatoR.Alternative.Lite.Tests.Helpers.Handlers
{
    public class TestHandler : IRequestHandler<TestRequest, TestResponse>
    {
        public Task<Result<TestResponse>> Handle(TestRequest request, CancellationToken ct)
        {
            return Task.FromResult(Result.Ok(new TestResponse()));
        }
    }
}