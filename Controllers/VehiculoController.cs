using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_MadonnaTizianoLab4.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;


namespace FinalMadonnaTizianoLab4.Controllers
{
    [Authorize]

    public class VehiculoController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment env;

        public VehiculoController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }
        public async Task<IActionResult> EliminarTodo()
        {
            _context.Vehiculos.RemoveRange(_context.Vehiculos);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Importar()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivoCSV = archivos[0];
                if (archivoCSV.Length > 0)
                {
                    var pathDestino = Path.Combine(env.WebRootPath, "importacionesVehiculos");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoCSV.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                    using (var fileStream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivoCSV.CopyTo(fileStream);
                    };

                    var fileReader = new FileStream(rutaDestino, FileMode.Open);
                    List<string> files = new List<string>();
                    List<Vehiculo> LibroFiles = new List<Vehiculo>();

                    StreamReader fileContent = new StreamReader(fileReader, System.Text.Encoding.Default);
                    do
                    {
                        files.Add(fileContent.ReadLine());
                    }
                    while (!fileContent.EndOfStream);

                    if (files.Count > 0)
                    {
                        foreach (var row in files)
                        {
                            string[] data = row.Split(';');
                            if (data.Length != 7)
                            {
                                Vehiculo stock = new Vehiculo();
                                stock.NameMatricula = data[0].Trim();
                                LibroFiles.Add(stock);
                            }
                        }
                        if (LibroFiles.Count() > 0)
                        {
                            _context.AddRange(LibroFiles);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            var appDBContext = _context.Vehiculos;
            return View("Index", await appDBContext.ToListAsync());
        }

        // GET: Vehiculo
        public async Task<IActionResult> Index(string? busqVehiculo, int? pageNumber, int pageSize = 8)
        {
            var appDBContext = _context.Vehiculos.Select(l => l);
            if (!string.IsNullOrEmpty(busqVehiculo))
            {
                appDBContext = appDBContext.Where(l => l.NameMatricula.ToString().Contains(busqVehiculo.ToString()));
            }
            int totalVehiculos = await _context.Vehiculos.CountAsync();

            int currentPage = pageNumber ?? 1;

            var pagedList = await appDBContext.Skip((currentPage - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalVehiculos / pageSize);

            return View(pagedList);
        }

        // GET: Vehiculo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // GET: Vehiculo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehiculo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameMatricula")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehiculo);
        }

        // GET: Vehiculo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            return View(vehiculo);
        }

        // POST: Vehiculo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameMatricula")] Vehiculo vehiculo)
        {
            if (id != vehiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.Id))
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
            return View(vehiculo);
        }

        // GET: Vehiculo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // POST: Vehiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.Id == id);
        }
    }
}
