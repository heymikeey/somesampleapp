using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySampleWebApp.Models; // For ErrorViewModel

namespace MySampleWebApp.Controllers;

// This controller will serve the main view for the SPA and can handle basic web app functions.
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger; // Logger for logging messages

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // This action will serve the main page (index.html) of your Angular application.
    // In a typical SPA setup, this might just return a View that contains the <app-root> tag.
    public IActionResult Index()
    {
        return View(); // Returns the default view (e.g., Views/Home/Index.cshtml)
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}