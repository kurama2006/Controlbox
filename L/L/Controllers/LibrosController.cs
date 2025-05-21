using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using L.Models;

namespace L.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ControlboxdbContext _context;

        public LibrosController(ControlboxdbContext context)
        {
            _context = context;
        }

        // GET: Libros
        public async Task<IActionResult> Index(string searchString, int? categoriaId)
        {
            // Corrige el nombre del campo del SelectList
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre");

            var libros = _context.Libros.Include(l => l.Categoria).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                libros = libros.Where(l => l.Titulo.Contains(searchString) || l.Autor.Contains(searchString));
            }

            if (categoriaId != null && categoriaId > 0)
            {
                libros = libros.Where(l => l.CategoriaId == categoriaId);
            }

            return View(await libros.ToListAsync());

        }
        public async Task<IActionResult> IndexUser(string searchString, int? categoriaId)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre");

            // Incluir las reseñas al consultar los libros
            var libros = _context.Libros
                                 .Include(l => l.Categoria)
                                 .Include(l => l.Reseña) // ¡Añade esta línea!
                                 .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                libros = libros.Where(l => l.Titulo.Contains(searchString) || l.Autor.Contains(searchString));
            }

            if (categoriaId != null && categoriaId > 0)
            {
                libros = libros.Where(l => l.CategoriaId == categoriaId);
            }

            return View(await libros.ToListAsync());
        }


        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(m => m.LibroId == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }
        // GET: Libros/Libro/5
        public async Task<IActionResult> Vista(int? id)
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var libro = await _context.Libros
                .Include(l => l.Categoria)
                .Include(l => l.Reseña)
                    .ThenInclude(r => r.Usuario)
                .FirstOrDefaultAsync(l => l.LibroId == id);

            if (libro == null)
            {
                return NotFound();
            }

                

            ViewBag.LibroId = id;
            return View(libro); // Este busca la vista Views/Libros/Libro.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Vista(int LibroId, string Contenido, int Calificacion)
        {
            // Aquí puedes obtener el ID del usuario autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId"); // O como tengas autenticado el usuario

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuarios"); // O manejar como prefieras
            }

            var nuevaResena = new Reseña
            {
                LibroId = LibroId,
                UsuarioId = usuarioId.Value,
                Comentario = Contenido,
                Calificacion = (sbyte)Calificacion,
                FechaReseña = DateTime.Now
            };

            _context.Reseña.Add(nuevaResena);
            await _context.SaveChangesAsync();

            return RedirectToAction("Vista", new { id = LibroId });
        }



        // GET: Libros/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre");
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibroId,Titulo,Autor,CategoriaId,Resumen,FechaPublicacion,FechaRegistro,UrlImagen")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                await _context.SaveChangesAsync();

                // Opcional: redirige a los detalles del libro creado
                return RedirectToAction("Details", new { id = libro.LibroId });
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", libro.CategoriaId);
            return View(libro);
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", libro.CategoriaId);
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibroId,Titulo,Autor,CategoriaId,Resumen,FechaPublicacion,FechaRegistro,UrlImagen")] Libro libro)
        {
            if (id != libro.LibroId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.LibroId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", libro.CategoriaId);
            return View(libro);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(m => m.LibroId == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.LibroId == id);
        }
    }
}

