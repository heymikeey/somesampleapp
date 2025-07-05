using Microsoft.EntityFrameworkCore;
using MySampleApi.Models; // Required for the Item model

namespace MySampleApi.Data;

// DbContext is the main class that coordinates Entity Framework functionality
// for a given data model.
public class AppDbContext : DbContext
{
    // Constructor that accepts DbContextOptions, which will be provided by dependency injection
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet represents a collection of all entities in the context, or that can be queried from the database.
    // It maps to a table named "Items" in your SQL database.
    public DbSet<Item> Items { get; set; }

    // Optional: Configure model properties using the Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Example: Configure the 'Name' property to be required and have a max length
        modelBuilder.Entity<Item>()
            .Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Seed some initial data (optional)
        modelBuilder.Entity<Item>().HasData(
            new Item { Id = 1, Name = "Sample Item 1", Description = "This is the first sample item." },
            new Item { Id = 2, Name = "Sample Item 2", Description = "This is the second sample item." }
        );
    }
}