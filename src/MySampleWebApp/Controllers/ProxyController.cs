using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MySampleWebApp.Controllers;

[ApiController]
[Route("api/[controller]")] // Route for this proxy controller (e.g., /api/proxy)
public class ProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory; // Factory to create HttpClients
    private readonly ILogger<ProxyController> _logger; // Logger for logging messages

    public ProxyController(IHttpClientFactory httpClientFactory, ILogger<ProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Proxies GET requests to the MySampleApi.
    /// </summary>
    /// <param name="path">The path segment to append to the MySampleApi base URL.</param>
    /// <returns>The response from MySampleApi.</returns>
    [HttpGet("{*path}")] // Catches all GET requests to /api/proxy/{*path}
    public async Task<IActionResult> Get(string path)
    {
        _logger.LogInformation($"Proxying GET request to MySampleApi path: /{path}");
        var client = _httpClientFactory.CreateClient("MySampleApi"); // Get the named HttpClient
        var response = await client.GetAsync(path); // Make the request to MySampleApi

        // Read the content and return it to the client
        var content = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            Content = content,
            ContentType = response.Content.Headers.ContentType?.ToString(),
            StatusCode = (int)response.StatusCode
        };
    }

    /// <summary>
    /// Proxies POST requests to the MySampleApi.
    /// </summary>
    /// <param name="path">The path segment to append to the MySampleApi base URL.</param>
    /// <returns>The response from MySampleApi.</returns>
    [HttpPost("{*path}")] // Catches all POST requests to /api/proxy/{*path}
    public async Task<IActionResult> Post(string path)
    {
        _logger.LogInformation($"Proxying POST request to MySampleApi path: /{path}");
        var client = _httpClientFactory.CreateClient("MySampleApi");

        // Read the request body from the incoming request
        using var requestStream = new MemoryStream();
        await Request.Body.CopyToAsync(requestStream);
        requestStream.Seek(0, SeekOrigin.Begin); // Reset stream position

        var requestContent = new StreamContent(requestStream);
        // Copy content type from original request
        requestContent.Headers.ContentType = Request.ContentType != null ?
                                            new System.Net.Http.Headers.MediaTypeHeaderValue(Request.ContentType) : null;

        var response = await client.PostAsync(path, requestContent);

        var content = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            Content = content,
            ContentType = response.Content.Headers.ContentType?.ToString(),
            StatusCode = (int)response.StatusCode
        };
    }

    // You can add more proxy methods for PUT, DELETE, etc., as needed.
}