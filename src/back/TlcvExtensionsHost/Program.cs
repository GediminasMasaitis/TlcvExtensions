using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Services;
using Serilog.Events;
using TlcvExtensionsHost.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using TlcvExtensionsHost;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:5210");
builder.Services.Configure<ServiceConfig>(builder.Configuration);
builder.Services.AddTlcvExtensions();
builder.Logging.AddSerilog(CreateLogger());

var app = builder.Build();

app.MapGet("/query",
    (EngineManager engineManager) => new QueryResponse(engineManager.GetCurrentInfos()));

app.MapPost("/fen",
    async ([FromBody] FenRequest request, EngineManager engineManager, ILogger<Program> logger) =>
    {
        logger.LogInformation("FEN: {Fen}", request.Fen);
        await engineManager.SetFenAsync(request.Fen);

        return new FenResponse(engineManager.Engines.ConvertAll(x => x.Config));
    });

app.Lifetime.ApplicationStarted.Register(() => app.Services.GetRequiredService<EngineManager>().Run());
app.Lifetime.ApplicationStopping.Register(() => StopEngineProcesses(app));

await app.RunAsync();

Console.WriteLine("Graceful shutdown completed");

static Logger CreateLogger()
{
    var loggerConfiguration = new LoggerConfiguration();
    loggerConfiguration.Enrich.FromLogContext();
    loggerConfiguration.WriteTo.Console();
    loggerConfiguration.MinimumLevel.Debug();
    loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
    //loggerConfiguration.MinimumLevel.Information();
    //loggerConfiguration.MinimumLevel.Warning();
    return loggerConfiguration.CreateLogger();
}

static void StopEngineProcesses(WebApplication app)
{
    var shutDownTasks = app.Services.GetRequiredService<EngineManager>().Engines.Select(e => e.ShutDown());

    Task.WhenAll(shutDownTasks).Wait();
}
