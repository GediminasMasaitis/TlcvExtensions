using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace TlcvExtensionsHost
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddTlcvExtensions(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });
            services.AddControllers();

            services.AddTlcvExtensionsConfigs();

            services.AddSingleton<IEngineManager, EngineManager>();
            services.AddTransient<IEngine, Engine>();

            return services;
        }

        private static IServiceCollection AddTlcvExtensionsConfigs(this IServiceCollection services)
        {
            services.AddSingleton<ServiceConfig>(provider => provider.GetRequiredService<IOptions<ServiceConfig>>().Value);
            return services;
        }
    }
}
