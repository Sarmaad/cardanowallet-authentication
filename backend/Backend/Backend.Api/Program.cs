using Serilog.Events;
using Serilog;
using Backend.Api.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Filter.ByExcluding("EndsWith(RequestPath,'/health')")
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting host application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddEnvironmentVariables();
    builder.Host.UseSerilog();

    builder.ApiStandard();

    builder.Services.AddSingleton<IRepository, MemoryRepository>();
    builder.Services.AddTransient<ISessionService, SessionService>();

    var app = builder.Build();

    app.AppStandard();


    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
}



