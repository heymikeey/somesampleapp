using Microsoft.AspNetCore.Mvc;
using MySampleApi.Data;
using MySampleApi.Models;
using Microsoft.EntityFrameworkCore;
using MySampleApi.Services; // Required for IServiceBusSenderService

namespace MySampleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IServiceBusSenderService _serviceBusSenderService; // Inject Service Bus sender

    // Constructor for dependency injection
    public HelloController(ILogger<HelloController> logger, AppDbContext dbContext,
                           IServiceBusSenderService serviceBusSenderService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _serviceBusSenderService = serviceBusSenderService; // Assign the injected Service Bus sender
    }

    /// <summary>
    /// Gets a simple greeting message.
    /// </summary>
    /// <returns>A string greeting.</returns>
    [HttpGet(Name = "GetHello")]
    public IActionResult Get()
    {
        _logger.LogInformation("Hello endpoint was hit!");
        return Ok("Hello from the .NET 8 API!");
    }

    /// <summary>
    /// Posts a message and returns it.
    /// </summary>
    /// <param name="message">The message to echo back.</param>
    /// <returns>The echoed message.</returns>
    [HttpPost(Name = "PostMessage")]
    public IActionResult Post([FromBody] string message)
    {
        _logger.LogInformation($"Received message: {message}");
        return Ok($"API received your message: {message}");
    }

    /// <summary>
    /// Gets all items from the database.
    /// </summary>
    /// <returns>A list of Item objects.</returns>
    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        _logger.LogInformation("Getting all items from the database.");
        var items = await _dbContext.Items.ToListAsync();
        return Ok(items);
    }

    /// <summary>
    /// Adds a new item to the database.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The created item.</returns>
    [HttpPost("items")]
    public async Task<ActionResult<Item>> PostItem([FromBody] Item item)
    {
        if (item == null)
        {
            return BadRequest("Item cannot be null.");
        }

        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Added new item: {item.Name}");
        return CreatedAtAction(nameof(GetItems), new { id = item.Id }, item);
    }

    /// <summary>
    /// Sends a message to Azure Service Bus.
    /// </summary>
    /// <param name="messageContent">The content of the message to send.</param>
    /// <returns>A confirmation message.</returns>
    [HttpPost("send-message")] // New endpoint for sending Service Bus messages
    public async Task<IActionResult> SendServiceBusMessage([FromBody] string messageContent)
    {
        if (string.IsNullOrWhiteSpace(messageContent))
        {
            return BadRequest("Message content cannot be empty.");
        }

        _logger.LogInformation($"Attempting to send message to Service Bus: {messageContent}");
        try
        {
            await _serviceBusSenderService.SendMessageAsync(messageContent);
            _logger.LogInformation("Message sent to Service Bus successfully.");
            return Ok($"Message '{messageContent}' sent to Service Bus.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to Service Bus.");
            return StatusCode(500, $"Failed to send message to Service Bus: {ex.Message}");
        }
    }
}