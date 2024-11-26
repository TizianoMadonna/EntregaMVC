using Final_MadonnaTizianoLab4.Models;
using FinalMadonnaTizianoLab4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Drawing.Printing;
using X.PagedList.Extensions;

namespace FinalMadonnaTizianoLab4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? busqCodigo,int? pageNumber, int pageSize = 6)
        {
            var appDBcontext = _context.Entregas
                .Include(l => l.LocalidadReferencial)
                .Include(l => l.ConductorReferencial)
                .Include(l => l.VehiculoReferencial)
                .OrderByDescending(l => l.Id)
                .Take(100);

            if(busqCodigo.HasValue)
            {
                appDBcontext = appDBcontext.Where(l => l.Id.ToString() == busqCodigo.ToString());
            }


            int totalEntregas = await _context.Entregas.CountAsync();

            int currentPage = pageNumber ?? 1;
            var pagedList = await appDBcontext.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEntregas / pageSize);

            return View(pagedList);

            //return View(entregas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
