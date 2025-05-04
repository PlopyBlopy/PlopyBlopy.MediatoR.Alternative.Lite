using MediatoR.Alternative.Lite.Tests.Helpers.Handlers;
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
            services.AddMediatorAlt(); // Вызываем из теста → GetCallingAssembly() = тестовая сборка

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
    }
}