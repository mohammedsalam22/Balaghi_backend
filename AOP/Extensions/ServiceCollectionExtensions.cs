using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using AOP.Performance;

namespace AOP.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Castle DynamicProxy infrastructure for AOP
        /// </summary>
        public static IServiceCollection AddAopInterceptors(this IServiceCollection services)
        {
            // Register ProxyGenerator as singleton (thread-safe)
            services.AddSingleton<IProxyGenerator, ProxyGenerator>();

            // Register interceptors as both concrete type and IInterceptor
            services.AddScoped<PerformanceInterceptor>();
            services.AddScoped<IInterceptor>(sp => sp.GetRequiredService<PerformanceInterceptor>());

            return services;
        }

        /// <summary>
        /// Registers a service with performance interception
        /// </summary>
        public static IServiceCollection AddProxiedScoped<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            // Register the concrete implementation
            services.AddScoped<TImplementation>();

            // Register the proxied interface
            services.AddScoped<TInterface>(sp =>
            {
                var proxyGenerator = sp.GetRequiredService<IProxyGenerator>();
                var target = sp.GetRequiredService<TImplementation>();
                
                // Get all registered interceptors
                var interceptors = sp.GetServices<IInterceptor>().ToArray();

                if (interceptors.Length == 0)
                {
                    // No interceptors, return the target directly
                    return target;
                }

                // Create proxy with interceptors
                return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(
                    target,
                    interceptors);
            });

            return services;
        }

        /// <summary>
        /// Registers a concrete service with performance interception
        /// Note: This only works if TService has virtual methods
        /// </summary>
        public static IServiceCollection AddProxiedScoped<TService>(
            this IServiceCollection services)
            where TService : class
        {
            // First register the service normally so dependencies can be injected
            services.AddScoped<TService>(sp =>
            {
                // Create the actual instance with all dependencies
                var instance = ActivatorUtilities.CreateInstance<TService>(sp);
                
                var proxyGenerator = sp.GetRequiredService<IProxyGenerator>();
                var interceptors = sp.GetServices<IInterceptor>().ToArray();

                if (interceptors.Length == 0)
                {
                    // No interceptors, return instance as-is
                    return instance;
                }

                // Create proxy with the already-instantiated target
                // Note: This requires TService methods to be virtual
                return proxyGenerator.CreateClassProxyWithTarget(
                    instance,
                    interceptors);
            });

            return services;
        }
    }
}

