using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using L.Models;
using L.ViewModels;

namespace L.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ControlboxdbContext _context;

        public UsuariosController(ControlboxdbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {

            return View(await _context.Usuarios.Include(u => u.Rol).ToListAsync());

        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nombre,Correo,Contrasena,FechaRegistro,RolId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }


        // GET: Usuarios/Login
        public IActionResult Login()
        {
            return View();
        }
        private Usuario ValidarUsuario(string correo, string contraseña)
        {
            return _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Correo == correo && u.Contrasena == contraseña);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = ValidarUsuario(model.Correo, model.Contraseña);

                if (usuario != null)
                {
                    // Guardar datos en sesión
                    HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
                    HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
                    HttpContext.Session.SetString("RolNombre", usuario.Rol.Nombre); // << AQUI

                    TempData["SuccessMessage"] = "Inicio de sesión exitoso.";
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = "Correo o contraseña incorrectos.";
            }

            return View(model);
        }



        // GET: Usuarios/Registro
        public IActionResult Registro()
        {
            return View();
        }


        // POST: Usuarios/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ✅ Verificar si el correo ya existe
            var correoExistente = await _context.Usuarios
                .AnyAsync(u => u.Correo == model.Correo);

            if (correoExistente)
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                return View(model);
            }

            // Crear nuevo usuario desde el ViewModel
            var nuevoUsuario = new Usuario
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Contrasena = model.Contrasena, // 🔐 En producción, encriptar esta contraseña
                FechaRegistro = DateTime.Now,
                RolId = 2
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Redirigir a login o inicio
            return RedirectToAction("Index", "Home");
        }


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nombre,Correo,Contrasena,FechaRegistro,RolId")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
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
            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
