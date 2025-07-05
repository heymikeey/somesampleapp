using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MySampleApi.Data;
using MySampleApi.Models;
using MySampleApi.Services; // Required for IServiceBusSenderService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the Azure Service Bus sender service
// The connection string and queue name will be read from configuration
builder.Services.AddSingleton<IServiceBusSenderService, ServiceBusSenderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// In development, ensure the database is created and migrations are applied on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();