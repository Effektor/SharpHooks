namespace SharpHooks.Models;

public class WebHookContext
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> QueryParameters { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}