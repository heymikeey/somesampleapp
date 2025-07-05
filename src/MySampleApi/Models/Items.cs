namespace MySampleApi.Models;

// Represents a simple item in the database
public class Item
{
    public int Id { get; set; } // Primary key
    public string Name { get; set; } = string.Empty; // Name of the item
    public string Description { get; set; } = string.Empty; // Description of the item
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // When the item was created
}