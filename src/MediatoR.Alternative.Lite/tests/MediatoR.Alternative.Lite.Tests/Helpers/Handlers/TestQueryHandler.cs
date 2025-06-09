using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;

namespace MediatoR.Alternative.Lite.Tests.Helpers.Handlers
{
    public class TestQueryHandler : IQueryHandler<TestQuery, TestResponse>
    {
        public Task<Result<TestResponse>> Handle(TestQuery request, CancellationToken ct)
        {
            return Task.FromResult(Result.Ok(new TestResponse()));
        }
    }
}