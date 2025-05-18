using Microsoft.AspNetCore.Mvc;
using RadiatorForecasting.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RadiatorForecasting.Data;


namespace RadiatorForecasting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

        //Уведомления для оператора
        private readonly ApplicationDbContext _context;

        [Authorize(Roles = "Оператор")]
        public async Task<IActionResult> Notifications()
        {
            var rejectedForecasts = await _context.ProductionFacts
                .Where(p => p.ForecastStatus == "Отклонён")
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(rejectedForecasts);
        }

    }
}
