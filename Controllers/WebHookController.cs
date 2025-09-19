using Microsoft.AspNetCore.Mvc;
using SharpHooks.Models;
using SharpHooks.Services;
using System.Text;

namespace SharpHooks.Controllers;

[ApiController]
public class WebHookController : ControllerBase
{
    private readonly ScriptingService _scriptingService;
    private readonly ILogger<WebHookController> _logger;
    private readonly WebHookConfig _config;

    public WebHookController(
        ScriptingService scriptingService, 
        ILogger<WebHookController> logger,
        WebHookConfig config)
    {
        _scriptingService = scriptingService;
        _logger = logger;
        _config = config;
    }

    [HttpPost]
    [HttpGet]
    [HttpPut]
    [HttpDelete]
    [HttpPatch]
    [Route("{*path}")]
    public async Task<IActionResult> HandleWebHook(string path)
    {
        var normalizedPath = "/" + path?.TrimStart('/');
        
        _logger.LogInformation("Received {Method} request for path: {Path}", Request.Method, normalizedPath);

        // Find matching webhook configuration
        var webhook = _config.Hooks.FirstOrDefault(h => h.Path.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase));
        if (webhook == null)
        {
            _logger.LogWarning("No webhook configuration found for path: {Path}", normalizedPath);
            return NotFound($"No webhook configured for path: {normalizedPath}");
        }

        try
        {
            // Create webhook context
            var context = await CreateWebHookContextAsync();
            context.Path = normalizedPath;
            context.Method = Request.Method;

            _logger.LogInformation("Executing script {Script} for webhook {Path}", webhook.Script, normalizedPath);

            // Execute the script
            var result = await _scriptingService.ExecuteScriptAsync(webhook.Script, context);

            _logger.LogInformation("Webhook {Path} processed successfully", normalizedPath);

            return Ok(new { 
                message = "Webhook processed successfully", 
                script = webhook.Script,
                result = result?.ToString() 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook {Path}", normalizedPath);
            return StatusCode(500, new { 
                message = "Error processing webhook", 
                error = ex.Message 
            });
        }
    }

    private async Task<WebHookContext> CreateWebHookContextAsync()
    {
        var context = new WebHookContext();

        // Copy headers
        foreach (var header in Request.Headers)
        {
            context.Headers[header.Key] = string.Join(", ", header.Value.ToArray());
        }

        // Copy query parameters
        foreach (var query in Request.Query)
        {
            context.QueryParameters[query.Key] = string.Join(", ", query.Value.ToArray());
        }

        // Read body
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        context.Body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        context.ContentType = Request.ContentType ?? string.Empty;

        return context;
    }
}