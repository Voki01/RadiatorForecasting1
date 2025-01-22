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

        //отображение страницы "Количество выпущенных"
        public IActionResult ManageReleasedProductions()
        {
            var productionFacts = _context.ProductionFacts
                .Select(f => new ReleasedProductionViewModel
                {
                    Id = f.Id,
                    Period = f.ForecastMonth,
                    AluminumReleased = _context.ReleasedProductions
                        .Where(r => r.ProductionFactId == f.Id)
                        .Select(r => r.AluminumQuantity)
                        .FirstOrDefault(),
                    CopperReleased = _context.ReleasedProductions
                        .Where(r => r.ProductionFactId == f.Id)
                        .Select(r => r.CopperQuantity)
                        .FirstOrDefault()
                }).ToList();

            return View(productionFacts);
        }


        [HttpPost]
        public IActionResult SaveReleasedProduction(int productionFactId, string material, int quantity)
        {
            var releasedProduction = _context.ReleasedProductions
                .FirstOrDefault(r => r.ProductionFactId == productionFactId);

            if (releasedProduction == null)
            {
                releasedProduction = new ReleasedProduction
                {
                    ProductionFactId = productionFactId,
                    AluminumQuantity = material == "Алюминий" ? quantity : 0,
                    CopperQuantity = material == "Медь" ? quantity : 0
                };
                _context.ReleasedProductions.Add(releasedProduction);
            }
            else
            {
                if (material == "Алюминий")
                    releasedProduction.AluminumQuantity += quantity;
                else if (material == "Медь")
                    releasedProduction.CopperQuantity += quantity;
            }

            _context.SaveChanges();

            return RedirectToAction("ManageReleasedProductions");
        }

        //Компания
        public IActionResult ManageCompanies()
        {
            var companies = _context.Companies.ToList();
            return View(companies);
        }
        //Добавление компании
        [HttpPost]
        public IActionResult AddCompany(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var newCompany = new Company { Name = name };
                _context.Companies.Add(newCompany);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageCompanies");
        }
        //Обновление компании
        [HttpPost]
        public IActionResult UpdateCompany(int id, string name)
        {
            var company = _context.Companies.FirstOrDefault(c => c.Id == id);
            if (company != null && !string.IsNullOrEmpty(name))
            {
                company.Name = name;
                _context.SaveChanges();
            }

            return RedirectToAction("ManageCompanies");
        }


        // Отображение страницы "Введение отделов"
        public IActionResult ManageDepartments()
        {
            var departments = _context.Departments
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    CompanyName = d.Company.Name
                })
                .ToList();

            ViewBag.Companies = _context.Companies.ToList(); // Для выпадающего списка
            return View(departments);
        }

        // Добавление нового отдела
        [HttpPost]
        public IActionResult AddDepartment(string departmentName, int companyId)
        {
            if (!string.IsNullOrEmpty(departmentName) && companyId > 0)
            {
                var department = new Department
                {
                    Name = departmentName,
                    CompanyId = companyId
                };

                _context.Departments.Add(department);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageDepartments");
        }



    }
}
