using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SharpHooks.Models;
using System.Linq;

namespace SharpHooks.Services;

public class ScriptingService
{
    private readonly ILogger<ScriptingService> _logger;
    private readonly string _scriptsPath;

    public ScriptingService(ILogger<ScriptingService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _scriptsPath = configuration["ScriptsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
        
        // Ensure scripts directory exists
        if (!Directory.Exists(_scriptsPath))
        {
            Directory.CreateDirectory(_scriptsPath);
            _logger.LogInformation("Created scripts directory at: {ScriptsPath}", _scriptsPath);
        }
    }

    public async Task<object?> ExecuteScriptAsync(string scriptName, WebHookContext context)
    {
        try
        {
            var scriptPath = Path.Combine(_scriptsPath, scriptName);
            
            if (!File.Exists(scriptPath))
            {
                _logger.LogWarning("Script file not found: {ScriptPath}", scriptPath);
                return null;
            }

            var scriptContent = await File.ReadAllTextAsync(scriptPath);
            _logger.LogInformation("Executing script: {ScriptName}", scriptName);

            var options = ScriptOptions.Default
                .WithReferences(
                    typeof(object).Assembly, 
                    typeof(Console).Assembly, 
                    typeof(Dictionary<,>).Assembly,
                    typeof(Enumerable).Assembly)
                .WithImports("System", "System.IO", "System.Threading.Tasks", "System.Collections.Generic", "System.Linq");

            var script = CSharpScript.Create(scriptContent, options, typeof(WebHookContext));
            var result = await script.RunAsync(context);

            _logger.LogInformation("Script executed successfully: {ScriptName}", scriptName);
            return result.ReturnValue;
        }
        catch (CompilationErrorException ex)
        {
            _logger.LogError(ex, "Compilation error in script {ScriptName}: {Errors}", scriptName, string.Join(", ", ex.Diagnostics));
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing script {ScriptName}", scriptName);
            throw;
        }
    }
}