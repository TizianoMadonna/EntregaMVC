using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_MadonnaTizianoLab4.Models;
using Microsoft.AspNetCore.Authorization;

namespace FinalMadonnaTizianoLab4.Controllers
{
    [Authorize]

    public class EntregaDetalleController : Controller
    {
        private readonly AppDBContext _context;

        public EntregaDetalleController(AppDBContext context)
        {
            _context = context;
        }

        // GET: EntregaDetalle
        public async Task<IActionResult> Index(string? busqCodigo, string? busqProducto, int? pageNumber, int pageSize = 8)
        {
            var appDBContext = _context.EntregaDetalles
                .Include(e => e.EntregaReferencial)
                .Include(e => e.ProductoReferencial)
                .Take(50)
                .Select(l => l);
            if (!string.IsNullOrEmpty(busqCodigo))
            {
                appDBContext = appDBContext.Where(l => l.EntregaId.ToString().Contains(busqCodigo.ToString()));
            }
            if (!string.IsNullOrEmpty(busqProducto))
            {
                appDBContext = appDBContext.Where(l=>l.ProductoId.ToString().Contains(busqProducto.ToString()));
            }
            int totalDetalles = await _context.EntregaDetalles.CountAsync();

            int currentPage = pageNumber ?? 1;

            var pagedList = await appDBContext.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalDetalles / pageSize);

            return View(pagedList);
        }

        // GET: EntregaDetalle/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaDetalle = await _context.EntregaDetalles
                .Include(e => e.EntregaReferencial)
                .Include(e => e.ProductoReferencial)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entregaDetalle == null)
            {
                return NotFound();
            }

            return View(entregaDetalle);
        }

        // GET: EntregaDetalle/Create
        public IActionResult Create(int? entregaId)
        {
            ViewData["EntregaId"] = new SelectList(_context.Entregas, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "NameProducto");
            var model = new EntregaDetalle();
            if (entregaId.HasValue)
            {
                model.EntregaId = entregaId.Value;
            }
            return View(model);
        }

        // POST: EntregaDetalle/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,EntregaId,ProductoId,CantidadCajas")] EntregaDetalle entregaDetalle)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(entregaDetalle);
        //        await _context.SaveChangesAsync();
        //        //return RedirectToAction(nameof(Index));
        //        return RedirectToAction("Create", new { entregaId = entregaDetalle.EntregaId });

        //    }

        //    ViewData["EntregaId"] = new SelectList(_context.Entregas, "Id", "Id", entregaDetalle.EntregaId);
        //    ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "NameProducto", entregaDetalle.ProductoId);
        //    return View(entregaDetalle);
        //}
        public async Task<IActionResult> Create([Bind("Id,EntregaId,ProductoId,CantidadCajas")] EntregaDetalle entregaDetalle)
        {
            if (ModelState.IsValid)
            {
                var entrega = await _context.Entregas
                                             .Include(e => e.Detalles)
                                             .FirstOrDefaultAsync(e => e.Id == entregaDetalle.EntregaId);

                if (entrega == null)
                {
                    return NotFound("La entrega especificada no existe.");
                }

                int sumaTotalCajas = entrega.Detalles.Sum(d => d.CantidadCajas) + entregaDetalle.CantidadCajas;
                int MaxCajas = 100;

                if (sumaTotalCajas > MaxCajas)
                {
                    ModelState.AddModelError("CantidadCajas", $"El camión solo entrega {MaxCajas} cajas de productos.");

                    ViewData["EntregaId"] = new SelectList(_context.Entregas, "Id", "Id", entregaDetalle.EntregaId);
                    ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "NameProducto", entregaDetalle.ProductoId);
                    return View(entregaDetalle);
                }
                _context.Add(entregaDetalle);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", new { entregaId = entregaDetalle.EntregaId });
            }

            ViewData["EntregaId"] = new SelectList(_context.Entregas, "Id", "Id", entregaDetalle.EntregaId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "NameProducto", entregaDetalle.ProductoId);
            return View(entregaDetalle);
        }

        // GET: EntregaDetalle/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaDetalle = await _context.EntregaDetalles.FindAsync(id);
            if (entregaDetalle == null)
            {
                return NotFound();
            }
            ViewData["EntregaId"] = new SelectList(_context.Entregas, "Id", "Id", entregaDetalle.EntregaId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "NameProducto", entregaDetalle.ProductoId);
            return View(entregaDetalle);
        }

        // POST: EntregaDetalle/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EntregaId,ProductoId,CantidadCajas")] EntregaDetalle entregaDetalle)
        {
            if (id != entregaDetalle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entregaDetalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntregaDetalleExists(entregaDetalle.Id))
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
            ViewData["EntregaId"] = new SelectList(_context.Entregas, "Id", "Id", entregaDetalle.EntregaId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "NameProducto", entregaDetalle.ProductoId);
            return View(entregaDetalle);
        }

        // GET: EntregaDetalle/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entregaDetalle = await _context.EntregaDetalles
                .Include(e => e.EntregaReferencial)
                .Include(e => e.ProductoReferencial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entregaDetalle == null)
            {
                return NotFound();
            }

            return View(entregaDetalle);
        }

        // POST: EntregaDetalle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entregaDetalle = await _context.EntregaDetalles.FindAsync(id);
            if (entregaDetalle != null)
            {
                _context.EntregaDetalles.Remove(entregaDetalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntregaDetalleExists(int id)
        {
            return _context.EntregaDetalles.Any(e => e.Id == id);
        }
    }
}
