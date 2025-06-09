using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatoR.Alternative.Lite.Tests.Unit
{
    public class RequestHandlerCtorDIInstanceTests
    {
        [Fact]
        public void RequestHandler_DIInstance_HasAdded()
        {
            // Arrange
            var assemblySource = @"
                using System;
                using MediatoR.Alternative.Lite;
                using FluentResults;
                using System.Threading;
                using System.Threading.Tasks;
                using MediatoR.Alternative.Lite.Tests.Helpers;

                public record TestRequest() : IRequest<TestResponse>;
                public record TestResponse();

                public class TestDIHandler : IRequestHandler<TestRequest, TestResponse>
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
            ";

            AssemblyCompiler assemblyCompiler = new AssemblyCompiler();

            Assembly[] assemblies =
            {
                typeof(object).Assembly,            // System.Private.CoreLib
                typeof(System.Runtime.GCSettings).Assembly,  // System.Runtime
                typeof(Task).Assembly,              // System.Threading.Tasks
                typeof(Enumerable).Assembly,        // System.Linq
                typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly, // Microsoft.CSharp
                Assembly.Load("netstandard, Version=2.1.0.0"),

                typeof(IRequest<>).Assembly,        // MediatoR.Alternative.Lite
                typeof(SomeOtherDependency).Assembly,
                typeof(Result).Assembly             // FluentResults
            };

            var assembly = assemblyCompiler.Compile("DynamicAssembly", assemblies, assemblySource);

            var services = new ServiceCollection();

            // Act
            services.AddScoped<SomeOtherDependency>();

            services.AddMediatorAlt(assembly);

            var provider = services.BuildServiceProvider();

            // Assert

            var handler = assembly.GetType("TestDIHandler");

            Assert.NotNull(handler);
        }
    }
}