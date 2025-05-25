using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;

namespace MediatoR.Alternative.Lite.Tests.Helpers.Handlers
{
    public class TestRequestQueryHandler : IRequestHandler<TestRequestQuery, TestResponse>
    {
        public Task<Result<TestResponse>> Handle(TestRequestQuery request, CancellationToken ct)
        {
            return Task.FromResult(Result.Ok(new TestResponse()));
        }
    }
}