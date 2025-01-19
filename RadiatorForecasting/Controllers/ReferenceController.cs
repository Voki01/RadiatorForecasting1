using Microsoft.AspNetCore.Mvc;
using RadiatorForecasting.Data;
using RadiatorForecasting.Models;
using System.Linq;

namespace RadiatorForecasting.Controllers
{
    public class ReferenceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReferenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Главная страница "Ведение справочников"
        public IActionResult Index()
        {
            return View();
        }

        // Подстраница "Введение текущих запасов"
        public IActionResult ManageStocks()
        {
            var stocks = _context.Stocks.ToList();
            return View(stocks);
        }

        // Добавление или обновление текущих запасов
        [HttpPost]
        public IActionResult UpdateStock(string material, int currentStock)
        {
            var stock = _context.Stocks.FirstOrDefault(s => s.Material == material);
            if (stock != null)
            {
                stock.CurrentStock = currentStock;
            }
            else
            {
                _context.Stocks.Add(new Stock
                {
                    Material = material,
                    CurrentStock = currentStock
                });
            }

            _context.SaveChanges();

            TempData["Message"] = "Запасы успешно обновлены.";
            return RedirectToAction("ManageStocks");
        }
    }
}
