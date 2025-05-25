using MediatoR.Alternative.Lite.Tests.Helpers.Handlers;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatoR.Alternative.Lite.Tests.Unit
{
    public class CommandQueryHandlersDIRegisterTests
    {
        [Fact]
        public void CommandHandler_DIRegister_IsRegister()
        {
            // Arrange

            var service = new ServiceCollection();
            var expectedAssembly = Assembly.GetExecutingAssembly();

            // Act

            // Scan the current build to find all objects inherited from IRequestHandler
            service.AddMediatorAlt();

            // Assert

            // Defining the Handler type
            var handlerType = typeof(IRequestHandler<TestCommand, TestResponse>);
            // Search for objects inherited from ICommandHandler
            var register = service.FirstOrDefault(s => s.ServiceType == handlerType);

            // Checking for the existence of an object
            Assert.NotNull(register);
            // Checking for type compliance
            Assert.Equal(typeof(TestCommandHandler), register.ImplementationType);
            // Checking for assembly compliance
            Assert.Equal(expectedAssembly, register.ImplementationType.Assembly);
        }

        [Fact]
        public void QueryHandler_DIRegister_IsRegister()
        {
            // Arrange

            var services = new ServiceCollection();
            var expectedAssembly = Assembly.GetExecutingAssembly();

            // Act

            // Scan the current build to find all objects inherited from IRequestHandler
            services.AddMediatorAlt();

            // Assert

            // Defining the Handler type
            var handlerType = typeof(IRequestHandler<TestQuery, TestResponse>);
            // Search for objects inherited from IQueryHandler
            var register = services.FirstOrDefault(s => s.ServiceType == handlerType);

            // Checking for the existence of an object
            Assert.NotNull(register);
            // Checking for type compliance
            Assert.Equal(typeof(TestQueryHandler), register.ImplementationType);
            // Checking for assembly compliance
            Assert.Equal(expectedAssembly, register.ImplementationType.Assembly);
        }
    }
}