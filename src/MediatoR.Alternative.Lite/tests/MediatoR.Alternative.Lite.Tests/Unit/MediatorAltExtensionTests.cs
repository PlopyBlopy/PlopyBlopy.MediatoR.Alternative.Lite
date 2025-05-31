using FluentResults;
using MediatoR.Alternative.Lite.Tests.Helpers;
using MediatoR.Alternative.Lite.Tests.Helpers.Handlers;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatoR.Alternative.Lite.Tests.Unit
{
    public class MediatorAltExtensionTests
    {
        [Fact]
        public void AddMediatorAlt_EqualAssembly_IsEqualAssemblies()
        {
            // Arrange
            var services = new ServiceCollection();
            var expectedAssembly = Assembly.GetExecutingAssembly(); // Тестовая сборка

            // Act
            services.AddMediatorAlt(); // Calling from the test → GetCallingAssembly() = test assembly

            // Assert
            var handlerType = typeof(IRequestHandler<TestRequest, TestResponse>);
            var registration = services.FirstOrDefault(s => s.ServiceType == handlerType);

            //Checking that the Handler is in the container
            Assert.NotNull(registration);
            //We check that TestHandler is registered for IRequestHandler<Test Request, Test Response>.
            Assert.Equal(typeof(TestHandler), registration.ImplementationType);
            //Making sure that the TestHandler type belongs to the test build.
            Assert.Equal(expectedAssembly, registration.ImplementationType.Assembly);
        }

        [Fact]
        public void AddMediatorAlt_EqualAssemblies_IsEqualAssemblies()
        {
            // Arrange
            var assemblySource1 = @"
                using MediatoR.Alternative.Lite;
                using FluentResults;
                using System.Threading;
                using System.Threading.Tasks;

                public record TestCommand1() : ICommand<TestCommandResponse1>;
                public record TestCommandResponse1();

                public class TestCommandHandler1 : ICommandHandler<TestCommand1, TestCommandResponse1>
                {
                    public Task<Result<TestCommandResponse1>> Handle(TestCommand1 request, CancellationToken ct)
                    {
                        return Task.FromResult(Result.Ok(new TestCommandResponse1()));
                    }
                }";

            var assemblySource2 = @"
                using MediatoR.Alternative.Lite;
                using FluentResults;
                using System.Threading;
                using System.Threading.Tasks;

                public record TestCommand2() : ICommand<TestCommandResponse2> {}
                public record TestCommandResponse2();
                public class TestCommandHandler2 : ICommandHandler<TestCommand2, TestCommandResponse2>
                {
                    public Task<Result<TestCommandResponse2>> Handle(TestCommand2 request, CancellationToken ct)
                    {
                        return Task.FromResult(Result.Ok(new TestCommandResponse2()));
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

                typeof(ICommand<>).Assembly,        // MediatoR.Alternative.Lite
                typeof(Result).Assembly             // FluentResults
            };

            var assembly1 = assemblyCompiler.Compile("DynamicAssembly1", assemblies, assemblySource1);
            var assembly2 = assemblyCompiler.Compile("DynamicAssembly2", assemblies, assemblySource2);

            var services = new ServiceCollection();

            // Act

            services.AddMediatorAlt(assembly1, assembly2);

            var provider = services.BuildServiceProvider();

            // Assert

            // Check first handler
            var handlerType1 = assembly1.GetType("TestCommandHandler1");

            Assert.NotNull(handlerType1);

            var handlerInterface1 = handlerType1.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            var handler1 = provider.GetService(handlerInterface1);
            Assert.NotNull(handler1);
            Assert.IsType(handlerType1, handler1);

            // Check second handler
            var handlerType2 = assembly2.GetType("TestCommandHandler2");

            Assert.NotNull(handlerType2);

            var handlerInterface2 = handlerType2.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            var handler2 = provider.GetService(handlerInterface2);
            Assert.NotNull(handler2);
            Assert.IsType(handlerType2, handler2);
        }
    }
}