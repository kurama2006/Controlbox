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
    public class ReseñaController : Controller
    {
        private readonly ControlboxdbContext _context;

        public ReseñaController(ControlboxdbContext context)
        {
            _context = context;
        }

        // GET: Reseña
        public async Task<IActionResult> Index()
        {
            var controlboxdbContext = _context.Reseña.Include(r => r.Libro).Include(r => r.Usuario);
            return View(await controlboxdbContext.ToListAsync());
        }

        // GET: Reseña/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseña
                .Include(r => r.Libro)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReseñaId == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // GET: Reseña/Create
        public IActionResult Create()
        {
            ViewData["LibroId"] = new SelectList(_context.Libros, "LibroId", "LibroId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: Reseña/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reseña reseña)
        {
            if (ModelState.IsValid)
            {
                reseña.FechaReseña = DateTime.Now;
                _context.Add(reseña);
                await _context.SaveChangesAsync();

                // Redirige a la página del libro después de publicar
                return RedirectToAction("Libro", "Libros", new { id = reseña.LibroId });
            }

            return View(reseña); // Si hubo error, vuelve a mostrar el form
        }

        // GET: Reseña/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ModelState.Clear(); // Limpiar el ModelState

            var reseña = _context.Reseña.Include(r => r.Usuario).Include(r => r.Libro).FirstOrDefault(r => r.ReseñaId == id);

            if (reseña == null)
            {
                return NotFound();
            }

            ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "Nombre", reseña.UsuarioId);
            ViewBag.LibroId = new SelectList(_context.Libros, "LibroId", "Titulo", reseña.LibroId);

            return View(reseña);
        }

        // POST: Reseña/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReseñaId,UsuarioId,LibroId,Calificacion,Comentario,FechaReseña")] Reseña reseña)
        {
            if (id != reseña.ReseñaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reseña);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReseñaExists(reseña.ReseñaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            // Volver a cargar los SelectLists correctamente en caso de error de validación
            ViewBag.LibroId = new SelectList(_context.Libros, "LibroId", "Titulo", reseña.LibroId);
            ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "Nombre", reseña.UsuarioId);
            return View(reseña);
        }

        // GET: Reseña/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseña
                .Include(r => r.Libro)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReseñaId == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // POST: Reseña/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reseña = await _context.Reseña.FindAsync(id);
            if (reseña != null)
            {
                _context.Reseña.Remove(reseña);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReseñaExists(int id)
        {
            return _context.Reseña.Any(e => e.ReseñaId == id);
        }
    }
}
