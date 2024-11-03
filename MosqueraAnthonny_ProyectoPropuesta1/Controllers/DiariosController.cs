using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MosqueraAnthonny_ProyectoPropuesta1.Data;
using MosqueraAnthonny_ProyectoPropuesta1.Models;

namespace MosqueraAnthonny_ProyectoPropuesta1.Controllers
{
    public class DiariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiariosController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Diarios
        public async Task<IActionResult> Index(int? usuarioId)
        {
            if (usuarioId == null)
            {
                // Si no se proporciona un ID de usuario, solo se muestra la lista de diarios.
                var diarioContext = _context.Diarios.Include(d => d.Usuario);
                return View(await diarioContext.ToListAsync());
            }

            // Buscar el diario por ID de usuario.
            var diarioUsuario = await _context.Diarios.Where(d => d.UsuarioId == usuarioId).Include(d => d.Usuario).ToListAsync();

            if (diarioUsuario.Count == 0)
            {
                ViewBag.Mensaje = "No se encontró un diario para el ID de usuario proporcionado.";
                return View(await _context.Diarios.Include(d => d.Usuario).ToListAsync());
            }

            // Mostrar los diarios del usuario encontrado.
            return View(diarioUsuario);
        }


        // POST: Diarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiario(int usuarioId, string contenido)
        {
            // Verificar si el contenido no está vacío
            if (string.IsNullOrWhiteSpace(contenido))
            {
                ModelState.AddModelError("", "El contenido no puede estar vacío.");
                return RedirectToAction(nameof(CreatePrompt), new { usuarioId });
            }

            // Verificar si el usuario existe
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró un usuario con el ID proporcionado.");
                return RedirectToAction(nameof(Index));
            }

            // Crear el nuevo diario
            var diario = new Diario
            {
                UsuarioId = usuarioId,
                Contenido = contenido,
                FechaHora = DateTime.Now // Asegúrate de que tu modelo tenga esta propiedad
            };

            _context.Add(diario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { usuarioId });
        }


        public IActionResult CreatePrompt(int usuarioId)
        {
            // Verificar si el usuario existe
            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario == null)
            {
                ViewBag.Mensaje = "No se encontró un usuario con el ID proporcionado.";
                return RedirectToAction(nameof(Index));
            }

            // Si el usuario existe, redirigir a la vista para crear un diario
            ViewBag.UsuarioId = usuarioId; // Pasar el ID del usuario a la vista
            return View();
        }


































        // GET: Diarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diario = await _context.Diarios
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diario == null)
            {
                return NotFound();
            }

            return View(diario);
        }

        // GET: Diarios/Create POR DEFECTO
        /*  public IActionResult Create()
          {
              ViewData["UsuarioId"] = new SelectList(_context.Set<Usuario>(), "Id", "Id");
              return View();
          }*/



        // POST: Diarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,Contenido,FechaHora")] Diario diario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Set<Usuario>(), "Id", "Id", diario.UsuarioId);
            return View(diario);
        }

        // GET: Diarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diario = await _context.Diarios.FindAsync(id);
            if (diario == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Set<Usuario>(), "Id", "Id", diario.UsuarioId);
            return View(diario);
        }

        // POST: Diarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioId,Contenido,FechaHora")] Diario diario)
        {
            if (id != diario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiarioExists(diario.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Set<Usuario>(), "Id", "Id", diario.UsuarioId);
            return View(diario);
        }

        // GET: Diarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diario = await _context.Diarios
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diario == null)
            {
                return NotFound();
            }

            return View(diario);
        }

        // POST: Diarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diario = await _context.Diarios.FindAsync(id);
            if (diario != null)
            {
                _context.Diarios.Remove(diario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiarioExists(int id)
        {
            return _context.Diarios.Any(e => e.Id == id);
        }
    }
}
