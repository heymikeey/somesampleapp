using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http; // Required for HttpClient

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Changed from AddControllersWithViews to AddControllers as we are primarily hosting a SPA
builder.Services.AddControllers(); // Only for API controllers (like ProxyController)
builder.Services.AddEndpointsApiExplorer(); // For Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // For Swagger UI

// Configure HttpClient for calling MySampleApi
// This sets up a named HttpClient that can be injected into services/controllers
builder.Services.AddHttpClient("MySampleApi", client =>
{
    // IMPORTANT: Replace with the actual URL of your MySampleApi.
    // If running MySampleApi in Docker locally, it's http://localhost:8080
    // If deployed to Azure, it would be its public URL (e.g., from APIM or App Service)
    client.BaseAddress = new Uri(builder.Configuration["MySampleApi:BaseUrl"] ?? "http://localhost:8080/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed error pages in development
    app.UseSwagger(); // Enable Swagger middleware
    app.UseSwaggerUI(); // Enable Swagger UI
}
else
{
    app.UseExceptionHandler("/Home/Error"); // General error page in production
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); // Enforces secure connections
}

// IMPORTANT: Commented out app.UseHttpsRedirection() for easier local development with Angular proxy.
// In production, HTTPS should be handled by a reverse proxy or configured with certificates.
// app.UseHttpsRedirection(); // Original line

app.UseStaticFiles(); // Enables serving static files (like your Angular app's build output)

app.UseRouting(); // Enables routing middleware

app.UseAuthorization(); // Enables authorization middleware

// IMPORTANT: Removed app.MapControllerRoute for default MVC views.
// All non-API routes will now fall back to the SPA's index.html.
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers
app.MapControllers(); // Maps API controller routes (e.g., /api/proxy)

// IMPORTANT: This is where the Angular app will be served.
// This ensures that any unmatched routes are handled by Angular's index.html.
// This must come AFTER MapControllers() so API routes are handled first.
app.MapFallbackToFile("/sampleapp/{*path:nonfile}", "sampleapp/index.html");

app.Run();