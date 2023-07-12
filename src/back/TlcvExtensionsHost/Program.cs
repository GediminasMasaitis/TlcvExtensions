using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Services;
using Serilog;

namespace TlcvExtensionsHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.WriteTo.Console();
            //loggerConfiguration.MinimumLevel.Debug();
            //loggerConfiguration.MinimumLevel.Information();
            loggerConfiguration.MinimumLevel.Warning();
            Log.Logger = loggerConfiguration.CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://127.0.0.1:5210");
            builder.Services.Configure<ServiceConfig>(builder.Configuration);
            builder.Services.AddTlcvExtensions();
            var app = builder.Build();

            app.UseRouting();
            app.UseEndpoints(endpointConfiguration =>
            {
                endpointConfiguration.MapControllers();
            });

            var provider = app.Services;
            var engineManager = provider.GetRequiredService<IEngineManager>();
            await engineManager.RunAsync();

            await app.RunAsync();
        }
    }
}