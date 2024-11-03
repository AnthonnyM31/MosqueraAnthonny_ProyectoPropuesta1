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
        public async Task<IActionResult> Index()
        {
            var diarioContext = _context.Diarios.Include(d => d.Usuario);
            return View(await diarioContext.ToListAsync());
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

        public IActionResult Create(string contenido, int usuarioId)
        {
            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario == null) return NotFound("Usuario no encontrado.");

            var entrada = new Diario
            {
                UsuarioId = usuario.Id,
                Contenido = contenido,
                FechaHora = DateTime.Now
            };

            _context.Diarios.Add(entrada);
            _context.SaveChanges();

            usuario.Puntos += (usuario.Puntos + 1) % 5 == 0 ? 100 : 0;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


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
