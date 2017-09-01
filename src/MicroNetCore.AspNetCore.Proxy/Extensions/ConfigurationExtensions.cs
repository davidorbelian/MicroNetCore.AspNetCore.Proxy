using MicroNetCore.AspNetCore.Proxy.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroNetCore.AspNetCore.Proxy.Extensions
{
    /// <summary>
    ///     Configuration extension methods for Proxy.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        ///     Adds services and options for Proxy.
        /// </summary>
        /// <param name="services">
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the
        ///     service to.
        /// </param>
        /// <param name="configuration">
        ///     The <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" /> to take options
        ///     from.
        /// </param>
        /// <returns>
        ///     A reference to <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance after the
        ///     operation has completed.
        /// </returns>
        public static IServiceCollection AddProxy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ProxyMiddleware>();

            services.AddScoped(s => configuration.GetSection(nameof(ProxyOptions)).Get<ProxyOptions>());
            services.AddScoped<IProxy, Proxy>();

            return services;
        }

        /// <summary>
        ///     Adds a <see cref="T:MicroNetCore.AspNetCore.Proxy.Middleware.ProxyMiddleware" /> to the application's request
        ///     pipeline.
        /// </summary>
        /// <param name="app">The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> instance.</param>
        /// <returns>The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> instance.</returns>
        public static IApplicationBuilder UseProxy(this IApplicationBuilder app)
        {
            app.UseMiddleware<ProxyMiddleware>();

            return app;
        }
    }
}