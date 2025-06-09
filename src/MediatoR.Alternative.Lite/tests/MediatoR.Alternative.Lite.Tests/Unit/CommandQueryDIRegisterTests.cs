using MediatoR.Alternative.Lite.Tests.Helpers.Handlers;
using MediatoR.Alternative.Lite.Tests.Helpers.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatoR.Alternative.Lite.Tests.Unit
{
    public class CommandQueryDIRegisterTests
    {
        [Fact]
        public void Command_DIRegister_IsRegister()
        {
            // Arrange
            var services = new ServiceCollection();
            var expectedAssembly = Assembly.GetExecutingAssembly(); // Тестовая сборка

            // Act
            services.AddMediatorAlt(); // Calling from the test → GetCallingAssembly() = test assembly

            // Assert
            var handlerType = typeof(IRequestHandler<TestRequestCommand, TestResponse>);
            var registration = services.FirstOrDefault(s => s.ServiceType == handlerType);

            Assert.NotNull(registration);
            //We check that TestHandler is registered for IRequestHandler<TestCommand, Test Response>.
            Assert.Equal(typeof(TestRequestCommandHandler), registration.ImplementationType);
            //Making sure that the TestHandler type belongs to the test build.
            Assert.Equal(expectedAssembly, registration.ImplementationType.Assembly);
        }

        [Fact]
        public void Query_DIRegister_IsRegister()
        {
            // Arrange
            var services = new ServiceCollection();
            var expectedAssembly = Assembly.GetExecutingAssembly(); // Тестовая сборка

            // Act
            services.AddMediatorAlt(); // Calling from the test → GetCallingAssembly() = test assembly

            // Assert
            var handlerType = typeof(IRequestHandler<TestRequestQuery, TestResponse>);
            var registration = services.FirstOrDefault(s => s.ServiceType == handlerType);

            Assert.NotNull(registration);
            //We check that TestHandler is registered for IRequestHandler<TestQuery, Test Response>.
            Assert.Equal(typeof(TestRequestQueryHandler), registration.ImplementationType);
            //Making sure that the TestHandler type belongs to the test build.
            Assert.Equal(expectedAssembly, registration.ImplementationType.Assembly);
        }
    }
}