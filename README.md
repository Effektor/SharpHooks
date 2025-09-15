# SharpHooks

A Kestrel-powered webhooks listener that executes C# scripts using Roslyn scripting. Configure webhook endpoints in a `hooks.json` file, and each webhook will invoke a corresponding `.csx` file with access to the HTTP request data.

## Features

- **Configurable Webhooks**: Define webhook endpoints in `hooks.json`
- **Roslyn Scripting**: Execute C# scripts (`.csx` files) for webhook handling
- **HTTP Context Access**: Scripts have access to HTTP method, headers, query parameters, and request body
- **Docker Ready**: Includes Dockerfile for containerized deployment
- **Logging**: Built-in logging for webhook execution and debugging

## Configuration

Create a `hooks.json` file in the application root:

```json
{
  "hooks": [
    {
      "path": "/webhook/github",
      "script": "github-webhook.csx",
      "description": "GitHub webhook handler"
    },
    {
      "path": "/webhook/deploy",
      "script": "deploy-webhook.csx", 
      "description": "Deployment webhook handler"
    }
  ]
}
```

## Script Context

Each script has access to a `WebHookContext` with the following properties:
- `Method`: HTTP method (GET, POST, etc.)
- `Path`: Webhook path
- `Headers`: Dictionary of HTTP headers
- `QueryParameters`: Dictionary of query parameters
- `Body`: Request body as string
- `ContentType`: Content-Type header value

## Example Script

Create a `.csx` file in the `Scripts/` directory:

```csharp
// github-webhook.csx
Console.WriteLine($"GitHub webhook received!");
Console.WriteLine($"Method: {Method}");
Console.WriteLine($"Headers: {Headers.Count}");

if (!string.IsNullOrEmpty(Body))
{
    Console.WriteLine($"Body: {Body}");
}

return "Webhook processed successfully!";
```

## Running the Application

### Local Development
```bash
dotnet run
```

### Docker
```bash
# Build the image
docker build -t sharphooks .

# Run the container
docker run -p 8080:80 -v $(pwd)/hooks.json:/app/hooks.json -v $(pwd)/Scripts:/app/Scripts sharphooks
```

## API Endpoints

- `GET /` - Application info and configured hooks
- `GET /health` - Health check endpoint
- `[ANY] /webhook/*` - Webhook endpoints as configured in `hooks.json`

## Logging

The application logs webhook executions, script results, and any errors. Configure logging levels in `appsettings.json`.
