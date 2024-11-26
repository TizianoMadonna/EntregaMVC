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
    public class ConductorController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment env;

        public ConductorController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }
        public async Task<IActionResult> EliminarTodo()
        {
            _context.Conductores.RemoveRange(_context.Conductores);
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
                    var pathDestino = Path.Combine(env.WebRootPath, "importacionesConductores");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoCSV.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                    using (var fileStream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivoCSV.CopyTo(fileStream);
                    };

                    var fileReader = new FileStream(rutaDestino, FileMode.Open);
                    List<string> files = new List<string>();
                    List<Conductor> LibroFiles = new List<Conductor>();

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
                                Conductor stock = new Conductor();
                                stock.NameConductor = data[0].Trim();
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
            var appDBContext = _context.Conductores.Select(l => l);

            return View("Index", await appDBContext.ToListAsync());
        }

        // GET: Conductor
        public async Task<IActionResult> Index(string? busqConductor, int? pageNumber, int pageSize = 8)
        {
            var appDBcontext = _context.Conductores.Select(l => l);
            if (!string.IsNullOrEmpty(busqConductor))
            {
                appDBcontext = appDBcontext.Where(l => l.NameConductor.ToString().Contains(busqConductor.ToString()));
            }
            int totalConductores = await _context.Conductores.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await appDBcontext.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalConductores / pageSize);

            return View(pagedList);
        }

        // GET: Conductor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conductor = await _context.Conductores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conductor == null)
            {
                return NotFound();
            }

            return View(conductor);
        }

        // GET: Conductor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conductor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameConductor")] Conductor conductor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conductor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(conductor);
        }

        // GET: Conductor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conductor = await _context.Conductores.FindAsync(id);
            if (conductor == null)
            {
                return NotFound();
            }
            return View(conductor);
        }

        // POST: Conductor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameConductor")] Conductor conductor)
        {
            if (id != conductor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conductor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConductorExists(conductor.Id))
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
            return View(conductor);
        }

        // GET: Conductor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conductor = await _context.Conductores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conductor == null)
            {
                return NotFound();
            }

            return View(conductor);
        }

        // POST: Conductor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conductor = await _context.Conductores.FindAsync(id);
            if (conductor != null)
            {
                _context.Conductores.Remove(conductor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConductorExists(int id)
        {
            return _context.Conductores.Any(e => e.Id == id);
        }
    }
}
