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
                ViewBag.Mensaje = "Por favor ingrese un ID de usuario para buscar.";
                return View(new List<Diario>());
            }

            // Buscar el usuario
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                ViewBag.Mensaje = "No se encontró un usuario con el ID proporcionado.";
                return View(new List<Diario>());
            }

            // Buscar los diarios del usuario con el ID especificado
            var diariosUsuario = await _context.Diarios.Where(d => d.UsuarioId == usuarioId).Include(d => d.Usuario).ToListAsync();

            if (!diariosUsuario.Any())
            {
                ViewBag.Mensaje = "No se encontró un diario para el ID de usuario proporcionado.";
                return View(new List<Diario>());
            }

            // usaremos el viewbag para mostrar tanto el nombre como el id en el html
            ViewBag.NombreUsuario = usuario.NombreUsuario;
            ViewBag.IdUsuario = usuarioId;

            // mostrar los diarios del usuario encontrado
            return View(diariosUsuario);
        }




        // POST: Diarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiario(int usuarioId, string contenido)
        {
            // verificar si el contenido no está vacío
            if (string.IsNullOrWhiteSpace(contenido))
            {
                ModelState.AddModelError("", "El contenido no puede estar vacío.");
                return RedirectToAction(nameof(CreatePrompt), new { usuarioId });
            }

            // verificar si el usuario existe
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró un usuario con el ID proporcionado.");
                return RedirectToAction(nameof(Index));
            }

            // crear el nuevo diario con el id dele usuario, el contenido y la fecha y hora de ese momento
            var diario = new Diario
            {
                UsuarioId = usuarioId,
                Contenido = contenido,
                FechaHora = DateTime.Now 
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

            // se usará nuevamente el viewbag para mostrar y usar el id del usuario en el index
            ViewBag.UsuarioId = usuarioId; 
            return View();
        }



        [HttpPost]
        public IActionResult AgregarEntrada(int usuarioId, string contenido)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Agregar nueva entrada
            var nuevaEntrada = new Diario
            {
                UsuarioId = usuarioId,
                Contenido = contenido,
                FechaHora = DateTime.Now
            };
            _context.Diarios.Add(nuevaEntrada);

            // Contar el número de entradas del usuario
            var totalEntradas = _context.Diarios.Count(d => d.UsuarioId == usuarioId);

            // Verificar si se alcanzaron las cinco actualizaciones
            if (totalEntradas % 5 == 0)
            {
                usuario.Puntos += 100;
            }

            _context.SaveChanges();

          
            return RedirectToAction("Index", new { usuarioId });
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
