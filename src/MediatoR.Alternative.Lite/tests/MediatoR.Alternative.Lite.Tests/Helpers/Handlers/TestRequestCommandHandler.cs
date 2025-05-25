using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;

namespace MediatoR.Alternative.Lite.Tests.Helpers.Handlers
{
    public class TestRequestCommandHandler : IRequestHandler<TestRequestCommand, TestResponse>
    {
        public Task<Result<TestResponse>> Handle(TestRequestCommand request, CancellationToken ct)
        {
            return Task.FromResult(Result.Ok(new TestResponse()));
        }
    }
}