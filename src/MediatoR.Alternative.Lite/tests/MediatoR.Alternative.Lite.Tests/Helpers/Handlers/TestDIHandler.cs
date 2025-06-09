using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;

namespace MediatoR.Alternative.Lite.Tests.Helpers.Handlers
{
    internal class TestDIHandler : IRequestHandler<TestRequest, TestResponse>
    {
        private readonly SomeOtherDependency _dependency;

        public TestDIHandler(SomeOtherDependency dependency)
        {
            _dependency = dependency;
        }

        public Task<Result<TestResponse>> Handle(TestRequest request, CancellationToken ct)
        {
            if (_dependency == null)
                throw new NullReferenceException();

            return Task.FromResult(Result.Ok(new TestResponse()));
        }
    }
}