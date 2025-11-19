using Claims_System.Areas.Identity.Data;
using Claims_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Claims_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;         
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [Authorize(Roles = "Lecturer,HR")]

        public IActionResult LecturerDashboard()
        {
            var claims = _context.LecturerClaims.ToList();
            return View("LecturerDashboard", claims);
        }

        [Authorize(Roles = "Coordinator,HR")]
        public IActionResult CoordinatorDashboard()
        {
            return View("CoordinatorDashboard");
        }

        [Authorize(Roles = "Manager,HR")]
        public IActionResult ManagerDashboard()
        {
            return View("ManagerDashboard");
        }

        public IActionResult HRDashboard()
        {
            return View("HRDashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
