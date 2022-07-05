using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InterceptorExtensions
    {
        public static void AddInterceptorSingleton<TInterface, TImplementation, TInterceptor>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
            where TInterceptor : class, IAsyncInterceptor
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddSingleton<TImplementation>();
            services.TryAddTransient<TInterceptor>();
            services.AddSingleton(ProviderFactory<TInterface, TImplementation, TInterceptor>);
            //services.AddSingleton(provider =>
            //{
            //    var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
            //    var implementation = provider.GetRequiredService<TImplementation>();
            //    var interceptor = provider.GetRequiredService<TInterceptor>();
            //    return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
            //});
        }

        public static void AddInterceptorScoped<TInterface, TImplementation, TInterceptor>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
            where TInterceptor : class, IAsyncInterceptor
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddScoped<TImplementation>();
            services.TryAddTransient<TInterceptor>();
            services.AddScoped(ProviderFactory<TInterface, TImplementation, TInterceptor>);
        }

        public static void AddInterceptorTransient<TInterface, TImplementation, TInterceptor>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
            where TInterceptor : class, IAsyncInterceptor
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddTransient<TImplementation>();
            services.TryAddTransient<TInterceptor>();
            services.AddTransient(ProviderFactory<TInterface, TImplementation, TInterceptor>);
        }

        private static TInterface ProviderFactory<TInterface,TImplementation, TInterceptor>(IServiceProvider provider)
            where TInterface : class
            where TImplementation : class, TInterface
            where TInterceptor : class, IAsyncInterceptor
        {
            var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
            var implementation = provider.GetRequiredService<TImplementation>();
            var interceptor = provider.GetRequiredService<TInterceptor>();
            return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
        }
    }
}
