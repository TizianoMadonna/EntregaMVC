using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_MadonnaTizianoLab4.Models;
using FinalMadonnaTizianoLab4.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;

namespace FinalMadonnaTizianoLab4.Controllers
{
    [Authorize]

    public class EntregaController : Controller
    {
        private readonly AppDBContext _context;

        public EntregaController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Entrega
        public async Task<IActionResult> Index(int? busqCodigo, int? busqConductor, int? busqVehiculo, int? busqLocalidad, DateTime? busqFecha, int? pageNumber, int pageSize = 3)
        { 
            var appDBContext = _context.Entregas
                .Include(e => e.ConductorReferencial)
                .Include(e => e.LocalidadReferencial)
                .Include(e => e.VehiculoReferencial)
                .OrderByDescending(e => e.Id)
                .Take(100)
                .Select(e => e);
            if (busqCodigo.HasValue)
            {
                appDBContext = appDBContext.Where(l => l.Id.ToString() == busqCodigo.ToString());
            }

            if (busqConductor.HasValue)
            {
                appDBContext = appDBContext.Where(e => e.ConductorId.ToString().Contains(busqConductor.ToString()));
            }

            if (busqVehiculo.HasValue)
            {
                appDBContext = appDBContext.Where(e => e.VehiculoId.ToString().Contains(busqVehiculo.ToString()));
            }

            if (busqLocalidad.HasValue)
            {
                appDBContext = appDBContext.Where(e => e.LocalidadId.ToString().Contains(busqLocalidad.ToString()));
            }
            if (busqFecha.HasValue)
            {
                appDBContext = appDBContext.Where(e => e.FechaEntrega == busqFecha);
            }

            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "NameMatricula");

            ViewData["ConductorId"] = new SelectList(_context.Conductores, "Id", "NameConductor");

            ViewData["LocalidadId"] = new SelectList(_context.Localidades, "Id", "NameLocalidad");

            int totalEntregas = await _context.Entregas.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await appDBContext.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEntregas / pageSize);

            return View(pagedList);


        }

        // GET: Entrega/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrega = await _context.Entregas
                .Include(e => e.ConductorReferencial)
                .Include(e => e.LocalidadReferencial)
                .Include(e => e.LocalidadReferencial.ProvinciaReferencial)
                .Include(e => e.VehiculoReferencial)
                .Include(e => e.Detalles)
                .ThenInclude(d => d.ProductoReferencial)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entrega == null)
            {
                return NotFound();
            }

            return View(entrega);
        }

        // GET: Entrega/Create
        public IActionResult Create()
        {
            ViewData["ConductorId"] = new SelectList(_context.Conductores, "Id", "NameConductor");
            ViewData["LocalidadId"] = new SelectList(_context.Localidades, "Id", "NameLocalidad");
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "NameMatricula");
            return View();
        }

        // POST: Entrega/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ConductorId,VehiculoId,LocalidadId,FechaEntrega")] Entrega entrega)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entrega);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConductorId"] = new SelectList(_context.Conductores, "Id", "NameConductor", entrega.ConductorId);
            ViewData["LocalidadId"] = new SelectList(_context.Localidades, "Id", "NameLocalidad", entrega.LocalidadId);
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "NameMatricula", entrega.VehiculoId);
            return View(entrega);
        }

        // GET: Entrega/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrega = await _context.Entregas.FindAsync(id);
            if (entrega == null)
            {
                return NotFound();
            }
            ViewData["ConductorId"] = new SelectList(_context.Conductores, "Id", "NameConductor", entrega.ConductorId);
            ViewData["LocalidadId"] = new SelectList(_context.Localidades, "Id", "NameLocalidad", entrega.LocalidadId);
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "NameMatricula", entrega.VehiculoId);
            return View(entrega);
        }

        // POST: Entrega/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConductorId,VehiculoId,LocalidadId,FechaEntrega")] Entrega entrega)
        {
            if (id != entrega.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entrega);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntregaExists(entrega.Id))
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
            ViewData["ConductorId"] = new SelectList(_context.Conductores, "Id", "NameConductor", entrega.ConductorId);
            ViewData["LocalidadId"] = new SelectList(_context.Localidades, "Id", "NameLocalidad", entrega.LocalidadId);
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "NameMatricula", entrega.VehiculoId);
            return View(entrega);
        }

        // GET: Entrega/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrega = await _context.Entregas
                .Include(e => e.ConductorReferencial)
                .Include(e => e.LocalidadReferencial)
                .Include(e => e.VehiculoReferencial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrega == null)
            {
                return NotFound();
            }

            return View(entrega);
        }

        // POST: Entrega/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrega = await _context.Entregas.FindAsync(id);
            if (entrega != null)
            {
                _context.Entregas.Remove(entrega);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntregaExists(int id)
        {
            return _context.Entregas.Any(e => e.Id == id);
        }
    }
}
