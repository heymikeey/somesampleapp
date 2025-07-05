using Microsoft.AspNetCore.Mvc;
using MySampleApi.Data; // Required for DbContext
using MySampleApi.Models; // Required for the Item model
using Microsoft.EntityFrameworkCore; // Required for ToListAsync, etc.

namespace MySampleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;
    private readonly AppDbContext _dbContext; // Inject your DbContext

    // Constructor for dependency injection
    public HelloController(ILogger<HelloController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext; // Assign the injected DbContext
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
    [HttpGet("items")] // New endpoint for items
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        _logger.LogInformation("Getting all items from the database.");
        var items = await _dbContext.Items.ToListAsync(); // Retrieve all items
        return Ok(items);
    }

    /// <summary>
    /// Adds a new item to the database.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The created item.</returns>
    [HttpPost("items")] // New endpoint for adding items
    public async Task<ActionResult<Item>> PostItem([FromBody] Item item)
    {
        if (item == null)
        {
            return BadRequest("Item cannot be null.");
        }

        _dbContext.Items.Add(item); // Add the item to the context
        await _dbContext.SaveChangesAsync(); // Save changes to the database
        _logger.LogInformation($"Added new item: {item.Name}");
        return CreatedAtAction(nameof(GetItems), new { id = item.Id }, item); // Return 201 Created
    }
}