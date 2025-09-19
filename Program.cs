using SharpHooks.Models;
using SharpHooks.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<ScriptingService>();

// Load webhook configuration
var hooksConfigPath = Path.Combine(builder.Environment.ContentRootPath, "hooks.json");
WebHookConfig webhookConfig;

if (File.Exists(hooksConfigPath))
{
    var jsonContent = await File.ReadAllTextAsync(hooksConfigPath);
    webhookConfig = JsonSerializer.Deserialize<WebHookConfig>(jsonContent, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }) ?? new WebHookConfig();
}
else
{
    var tempProvider = builder.Services.BuildServiceProvider();
    var tempLogger = tempProvider.GetRequiredService<ILogger<Program>>();
    tempLogger.LogWarning("hooks.json not found at {Path}. Using empty configuration.", hooksConfigPath);
    webhookConfig = new WebHookConfig();
}

builder.Services.AddSingleton(webhookConfig);

// Add logging
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.MapControllers();

// Add a health check endpoint
app.MapGet("/", () => new { 
    message = "SharpHooks Webhook Listener", 
    hooks = webhookConfig.Hooks.Select(h => new { h.Path, h.Script, h.Description }) 
});

app.MapGet("/health", () => "OK");

var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
appLogger.LogInformation("SharpHooks started with {HookCount} configured hooks", webhookConfig.Hooks.Count);

app.Run();
