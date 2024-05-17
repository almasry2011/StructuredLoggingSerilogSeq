using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Reflection;


Log.Logger = new LoggerConfiguration()
    .Enrich.With(new ApplicationDetailsEnricher())
        .WriteTo.Console(new JsonFormatter())
          .MinimumLevel.Verbose()
        //.WriteTo.File("logs/app.txt")
        .WriteTo.Seq("http://localhost:6171/")
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .CreateLogger();

Log.Information("Starting Web host");

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Logging.AddSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();




public class ApplicationDetailsEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var applicationAssembly = Assembly.GetEntryAssembly();
        var name = applicationAssembly.GetName().Name;
        var version = applicationAssembly.GetName().Version;
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ApplicationName", name));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ApplicationVersion", version));
    }
}