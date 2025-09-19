// GitHub Webhook Handler Script
// Access the webhook context through the built-in variable

Console.WriteLine($"GitHub webhook received!");
Console.WriteLine($"Method: {Method}");
Console.WriteLine($"Path: {Path}");
Console.WriteLine($"Content-Type: {ContentType}");

// Log headers
Console.WriteLine("Headers:");
foreach (var header in Headers)
{
    Console.WriteLine($"  {header.Key}: {header.Value}");
}

// Log query parameters if any
if (QueryParameters.Any())
{
    Console.WriteLine("Query Parameters:");
    foreach (var query in QueryParameters)
    {
        Console.WriteLine($"  {query.Key}: {query.Value}");
    }
}

// Parse and process the body
if (!string.IsNullOrEmpty(Body))
{
    Console.WriteLine($"Body length: {Body.Length} characters");
    Console.WriteLine($"Body preview: {Body.Substring(0, Math.Min(200, Body.Length))}...");
    
    // You could parse JSON here, for example:
    // var payload = System.Text.Json.JsonDocument.Parse(Body);
    // var eventType = payload.RootElement.GetProperty("action").GetString();
    // Console.WriteLine($"Event type: {eventType}");
}

// Return a result
return "GitHub webhook processed successfully!";