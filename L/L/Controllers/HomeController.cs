using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using L.Models;
using Microsoft.EntityFrameworkCore;

namespace L.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ControlboxdbContext _context;

    public HomeController(ILogger<HomeController> logger, ControlboxdbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var nombreUsuario = HttpContext.Session.GetString("NombreUsuario");
        ViewBag.NombreUsuario = nombreUsuario;

        var libros = _context.Libros.ToList(); // Trae todos los libros
        return View(libros); // Los pasamos a la vista
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
