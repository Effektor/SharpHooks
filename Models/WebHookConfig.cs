namespace SharpHooks.Models;

public class WebHookConfig
{
    public List<WebHook> Hooks { get; set; } = new();
}

public class WebHook
{
    public string Path { get; set; } = string.Empty;
    public string Script { get; set; } = string.Empty;
    public string? Description { get; set; }
}