using Microsoft.AspNetCore.Mvc;
using RadiatorForecasting.Data; // Пространство имен для работы с контекстом базы данных
using RadiatorForecasting.Models; // Пространство имен моделей
using System.Linq;

namespace RadiatorForecasting.Controllers
{
    public class ManagerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Отображение заглушки при первой загрузке
            return View();
        }

        public IActionResult Main()
        {
            ViewData["Title"] = "Главная";

            // Загружаем все факты выпуска
            var productionFacts = _context.ProductionFacts.ToList();

            // Находим последний прогноз (следующий месяц)
            var nextMonth = DateTime.Now.AddMonths(1).ToString("MMMM yyyy");
            var lastForecast = productionFacts
                .FirstOrDefault(fact => fact.ForecastMonth == nextMonth);

            ViewBag.LastForecast = lastForecast;

            // Если есть прогноз, выполняем расчёт предупреждений
            if (lastForecast != null)
            {
                // Получаем текущие запасы
                int aluminumStock = lastForecast.CurrentAluminumStock ?? 0;
                int copperStock = lastForecast.CurrentCopperStock ?? 0;

                // Рассчитываем рекомендуемые запасы
                int recommendedAluminumStock = (lastForecast.MonthlyForecastAluminum ?? 0) * ProductionFact.MaterialPerAluminumUnit;
                int recommendedCopperStock = (lastForecast.MonthlyForecastCopper ?? 0) * ProductionFact.MaterialPerCopperUnit;

                // Рассчитываем нехватку запасов
                int aluminumShortage = Math.Max(0, recommendedAluminumStock - aluminumStock);
                int copperShortage = Math.Max(0, recommendedCopperStock - copperStock);

                // Рассчитываем, на сколько радиаторов хватит текущих запасов
                int aluminumRadiatorsPossible = aluminumStock / ProductionFact.MaterialPerAluminumUnit;
                int copperRadiatorsPossible = copperStock / ProductionFact.MaterialPerCopperUnit;

                // Формируем предупреждение
                if (aluminumShortage > 0 || copperShortage > 0)
                {
                    var warningMessage = "Предупреждение:\n";
                    if (aluminumShortage > 0)
                    {
                        warningMessage += $"Не хватает {aluminumShortage} кг алюминия для производства {lastForecast.MonthlyForecastAluminum ?? 0} радиаторов.\n";
                    }
                    if (copperShortage > 0)
                    {
                        warningMessage += $"Не хватает {copperShortage} кг меди для производства {lastForecast.MonthlyForecastCopper ?? 0} радиаторов.\n";
                    }
                    warningMessage += "Рекомендуется пополнить запасы.\n\n";
                    warningMessage += $"Текущих запасов алюминия хватит на производство {aluminumRadiatorsPossible} радиаторов.\n";
                    warningMessage += $"Текущих запасов меди хватит на производство {copperRadiatorsPossible} радиаторов.\n";

                    ViewBag.WarningMessage = warningMessage; // Передаем предупреждение во View
                }
            }

            return View(productionFacts);
        }

        public IActionResult GenerateReport()
        {
            ViewData["Title"] = "Формирование отчета";
            return View();
        }

        public IActionResult NetworkSettings()
        {
            ViewData["Title"] = "Настройки сети";
            return View();
        }

    }
}
