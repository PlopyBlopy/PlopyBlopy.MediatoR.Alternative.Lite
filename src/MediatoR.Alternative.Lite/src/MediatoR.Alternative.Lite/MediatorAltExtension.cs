﻿using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatoR.Alternative.Lite
{
    /// <summary>
    /// Provides extension methods for configuring the custom mediator implementation
    /// </summary>
    public static class MediatorAltExtension
    {
        /// <summary>
        /// Registers all request handlers from the executing assembly
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Configured service collection</returns>
        /// <remarks>
        /// Automatically discovers and registers:
        /// - All IRequestHandler<TRequest, TResponse> implementations
        /// - Handlers can implement multiple request handler interfaces
        /// </remarks>
        public static IServiceCollection AddMediatorAlt(this IServiceCollection services)
        {
            services.AddScoped<ISender, Sender>();
            var assembly = Assembly.GetCallingAssembly().GetTypes();

            // Scan current assembly for handler implementations
            var handlers = assembly.Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

            foreach (var handler in handlers)
            {
                var interfaces = handler.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                // Register each handler interface separately
                foreach (var @interface in interfaces)
                {
                    services.AddTransient(@interface, handler);
                }
            }

            return services;
        }

        /// <summary>
        /// Registers all request handlers from specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan for handler implementations</param>
        /// <returns>Configured service collection</returns>
        /// <remarks>
        /// <para>Automatically discovers and registers:</para>
        /// <list type="bullet">
        /// <item><description>All IRequestHandler&lt;TRequest, TResponse&gt; implementations in provided assemblies</description></item>
        /// <item><description>Handlers implementing multiple request handler interfaces</description></item>
        /// </list>
        /// <para>Allows explicit control over scanned assemblies when automatic discovery from executing assembly is insufficient.</para>
        /// </remarks>
        public static IServiceCollection AddMediatorAlt(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<ISender, Sender>();

            var handlers = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                .ToList();

            if (handlers.Any())
            {
                foreach (var handler in handlers)
                {
                    var interfaces = handler.GetInterfaces()
                        .Where(i => i.IsGenericType &&
                                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                    // Register each handler interface separately
                    foreach (var @interface in interfaces)
                    {
                        services.AddTransient(@interface, handler);
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// Adds logging pipeline behavior to the mediator
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection with logging behavior</returns>
        /// <remarks>
        /// - Registers LoggingBehavior globally for all requests
        /// - Typically should be first in the pipeline chain
        /// - Method name contains 'FluentValidation' but registers logging (potential misnaming)
        /// </remarks>
        public static IServiceCollection AddMediatorAltLogging(this IServiceCollection services)
            => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        /// <summary>
        /// Adds validation pipeline behavior to the mediator
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection with validation behavior</returns>
        /// <remarks>
        /// - Registers ValidationBehavior globally for all requests
        /// - Executes before the handler based on pipeline order
        /// - Requires validators to be registered separately
        /// </remarks>
        public static IServiceCollection AddMediatorAltFluentValidation(this IServiceCollection services)
            => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}