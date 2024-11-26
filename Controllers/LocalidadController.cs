using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_MadonnaTizianoLab4.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;

namespace FinalMadonnaTizianoLab4.Controllers
{
    [Authorize]

    public class LocalidadController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment env;

        public LocalidadController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }
        public async Task<IActionResult> EliminarTodo()
        {
            _context.Localidades.RemoveRange(_context.Localidades);
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
                    var pathDestino = Path.Combine(env.WebRootPath, "importacionesLocalidades");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoCSV.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                    using (var fileStream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivoCSV.CopyTo(fileStream);
                    };

                    var fileReader = new FileStream(rutaDestino, FileMode.Open);
                    List<string> files = new List<string>();
                    List<Localidad> LibroFiles = new List<Localidad>();

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
                                Localidad stock = new Localidad();
                                stock.NameLocalidad = data[0].Trim();
                                stock.ProvinciaId = int.Parse(data[1].Trim());
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
            var appDBContext = _context.Localidades.Include(l => l.ProvinciaReferencial);
            return View("Index", await appDBContext.ToListAsync());
        }

        // GET: Localidad
        public async Task<IActionResult> Index(string? busqLocalidad, int? busqProvincia, int? pageNumber, int pageSize = 8)
        {
            var appDBContext = _context.Localidades.Include(l => l.ProvinciaReferencial).Select(e => e);
            if (!string.IsNullOrWhiteSpace(busqLocalidad))
            {
                appDBContext = appDBContext.Where(e => e.NameLocalidad.ToString().Contains(busqLocalidad.ToString()));
            }
            if (busqProvincia.HasValue)
            {
                appDBContext = appDBContext.Where(e => e.ProvinciaId.ToString() == busqProvincia.ToString());
            }
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "NameProvincia");

            int totalLocalidades = await _context.Localidades.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await appDBContext.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalLocalidades / pageSize);

            return View(pagedList);

            //return View(await appDBContext.ToListAsync());
        }

        // GET: Localidad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var localidad = await _context.Localidades
                .Include(l => l.ProvinciaReferencial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (localidad == null)
            {
                return NotFound();
            }

            return View(localidad);
        }

        // GET: Localidad/Create
        public IActionResult Create()
        {
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "NameProvincia");
            return View();
        }

        // POST: Localidad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameLocalidad,ProvinciaId")] Localidad localidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(localidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "NameProvincia", localidad.ProvinciaId);
            return View(localidad);
        }

        // GET: Localidad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var localidad = await _context.Localidades.FindAsync(id);
            if (localidad == null)
            {
                return NotFound();
            }
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "NameProvincia", localidad.ProvinciaId);
            return View(localidad);
        }

        // POST: Localidad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameLocalidad,ProvinciaId")] Localidad localidad)
        {
            if (id != localidad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(localidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocalidadExists(localidad.Id))
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
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "NameProvincia", localidad.ProvinciaId);
            return View(localidad);
        }

        // GET: Localidad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var localidad = await _context.Localidades
                .Include(l => l.ProvinciaReferencial)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (localidad == null)
            {
                return NotFound();
            }

            return View(localidad);
        }

        // POST: Localidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var localidad = await _context.Localidades.FindAsync(id);
            if (localidad != null)
            {
                _context.Localidades.Remove(localidad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocalidadExists(int id)
        {
            return _context.Localidades.Any(e => e.Id == id);
        }
    }
}
