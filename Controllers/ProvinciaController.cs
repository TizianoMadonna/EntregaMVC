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

    public class ProvinciaController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment env;

        public ProvinciaController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }
        public async Task<IActionResult> EliminarTodo()
        {
            _context.Provincias.RemoveRange(_context.Provincias);
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
                    var pathDestino = Path.Combine(env.WebRootPath, "importacionesProvincias");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoCSV.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                    using (var fileStream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivoCSV.CopyTo(fileStream);
                    };

                    var fileReader = new FileStream(rutaDestino, FileMode.Open);
                    List<string> files = new List<string>();
                    List<Provincia> LibroFiles = new List<Provincia>();

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
                                Provincia stock = new Provincia();
                                stock.NameProvincia = data[0].Trim();
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
            var appDBContext = _context.Provincias;
            return View("Index", await appDBContext.ToListAsync());
        }
        // GET: Provincia
        public async Task<IActionResult> Index(string? busqProvincia, int? pageNumber, int pageSize = 8)
        {
            var AppDBContext = _context.Provincias.Select(l => l);

            if (!string.IsNullOrEmpty(busqProvincia))
            {
                AppDBContext = AppDBContext.Where(l => l.NameProvincia.ToString().Contains(busqProvincia.ToString()));
            }

            int totalProvincia = await _context.Provincias.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await AppDBContext.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProvincia / pageSize);

            return View(pagedList);

            //return View(await _context.Provincias.ToListAsync
            //return View(await AppDBContext.ToListAsync());
        }

        // GET: Provincia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provincia = await _context.Provincias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (provincia == null)
            {
                return NotFound();
            }

            return View(provincia);
        }

        // GET: Provincia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Provincia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameProvincia")] Provincia provincia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(provincia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(provincia);
        }

        // GET: Provincia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provincia = await _context.Provincias.FindAsync(id);
            if (provincia == null)
            {
                return NotFound();
            }
            return View(provincia);
        }

        // POST: Provincia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameProvincia")] Provincia provincia)
        {
            if (id != provincia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(provincia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinciaExists(provincia.Id))
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
            return View(provincia);
        }

        // GET: Provincia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provincia = await _context.Provincias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (provincia == null)
            {
                return NotFound();
            }

            return View(provincia);
        }

        // POST: Provincia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var provincia = await _context.Provincias.FindAsync(id);
            if (provincia != null)
            {
                _context.Provincias.Remove(provincia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProvinciaExists(int id)
        {
            return _context.Provincias.Any(e => e.Id == id);
        }
    }
}
