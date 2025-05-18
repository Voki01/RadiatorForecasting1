using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using RadiatorForecasting.Data; // Пространство имен для работы с контекстом базы данных
using RadiatorForecasting.Models; // Пространство имен моделей
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RadiatorForecasting.Controllers
{
    public class ManagerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ManagerDashboardController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _httpClientFactory = httpClientFactory;
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

        
        //Раздел формирования отчета 

        [HttpGet]
        public IActionResult GenerateReport()
        {
            ViewData["Title"] = "Формирование отчета";
            return View();
        }

        [HttpPost]
        public IActionResult GenerateReport(int startMonth, int startYear, int endMonth, int endYear)
        {
            // Проверка на выбор всех полей
            if (startMonth == 0 || startYear == 0 || endMonth == 0 || endYear == 0)
            {
                TempData["ErrorMessage"] = "Пожалуйста, выберите все поля для периода.";
                return RedirectToAction("GenerateReport");
            }

            // Создаём диапазон дат
            var startDate = new DateTime(startYear, startMonth, 1);
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            // Проверка корректности диапазона
            if (startDate > endDate)
            {
                TempData["ErrorMessage"] = "Начальный период не может быть позже конечного. Пожалуйста, выберите корректный диапазон.";
                return RedirectToAction("GenerateReport");
            }

            // Получаем данные из базы
            var productionFacts = _context.ProductionFacts
                .AsEnumerable()
                .Where(fact => DateTime.TryParse(fact.ForecastMonth, out var factDate) &&
                               factDate >= startDate && factDate <= endDate)
                .ToList();

            // Собираем предупреждения
            var warningsByPeriod = new Dictionary<string, List<string>>();
            foreach (var fact in productionFacts)
            {
                if (fact.MonthlyForecastAluminum.HasValue && fact.MonthlyForecastCopper.HasValue)
                {
                    var aluminumShortage = Math.Max(0, fact.RecommendedAluminumStock.GetValueOrDefault() - fact.CurrentAluminumStock.GetValueOrDefault());
                    var copperShortage = Math.Max(0, fact.RecommendedCopperStock.GetValueOrDefault() - fact.CurrentCopperStock.GetValueOrDefault());

                    if (aluminumShortage > 0 || copperShortage > 0)
                    {
                        var warnings = new List<string>();
                        if (aluminumShortage > 0)
                        {
                            warnings.Add($"Не хватает {aluminumShortage} кг алюминия для производства {fact.MonthlyForecastAluminum.Value} радиаторов.");
                        }
                        if (copperShortage > 0)
                        {
                            warnings.Add($"Не хватает {copperShortage} кг меди для производства {fact.MonthlyForecastCopper.Value} радиаторов.");
                        }
                        warnings.Add("Рекомендуется пополнить запасы.");
                        warnings.Add($"Текущих запасов алюминия хватит на производство {fact.CurrentAluminumStock.GetValueOrDefault() / ProductionFact.MaterialPerAluminumUnit} радиаторов.");
                        warnings.Add($"Текущих запасов меди хватит на производство {fact.CurrentCopperStock.GetValueOrDefault() / ProductionFact.MaterialPerCopperUnit} радиаторов.");

                        warningsByPeriod[fact.ForecastMonth] = warnings;
                    }
                }
            }

            // Генерация Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Отчёт");

                // Заголовок
                worksheet.Cell(1, 1).Value = "Отчёт о выпуске продукции";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Range(1, 1, 1, 9).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Заголовки таблицы
                var headers = new[]
                {
            "Период (месяц)", "Среднемесячная температура окружающей среды (°C)", "Цена (руб)",
            "Рыночная цена (руб)", "Скидка (%)", "Прогноз на месяц [алюминиевые/медные (шт)]",
            "Запасы [текущие/рекомендуемые (кг)]", "Количество выпущенных [алюминиевые/медные (шт)]"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(2, i + 1).Value = headers[i];
                    worksheet.Cell(2, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(2, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(2, i + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                // Данные таблицы
                for (int i = 0; i < productionFacts.Count; i++)
                {
                    var fact = productionFacts[i];
                    worksheet.Cell(i + 3, 1).Value = fact.ForecastMonth;
                    worksheet.Cell(i + 3, 2).Value = fact.AverageTemperature;
                    worksheet.Cell(i + 3, 3).Value = fact.Price;
                    worksheet.Cell(i + 3, 4).Value = fact.CompetitorPrice;
                    worksheet.Cell(i + 3, 5).Value = fact.Discount;
                    worksheet.Cell(i + 3, 6).Value = $"{fact.MonthlyForecastAluminum ?? 0} / {fact.MonthlyForecastCopper ?? 0}";
                    worksheet.Cell(i + 3, 7).Value = $"Ал: {fact.CurrentAluminumStock ?? 0} / {fact.RecommendedAluminumStock ?? 0}\n" +
                                                     $"Мед: {fact.CurrentCopperStock ?? 0} / {fact.RecommendedCopperStock ?? 0}";
                    worksheet.Cell(i + 3, 8).Value = $"{fact.AluminumRadiatorsProduced} / {fact.CopperRadiatorsProduced}";

                    // Выравнивание ячеек
                    for (int j = 1; j <= 8; j++)
                    {
                        worksheet.Cell(i + 3, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(i + 3, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }
                }

                // Добавляем предупреждения в конец отчета
                if (warningsByPeriod.Any())
                {
                    var warningStartRow = productionFacts.Count + 4;
                    worksheet.Cell(warningStartRow, 1).Value = "Предупреждения по периодам:";
                    worksheet.Cell(warningStartRow, 1).Style.Font.Bold = true;

                    int row = warningStartRow + 1;
                    foreach (var period in warningsByPeriod)
                    {
                        worksheet.Cell(row, 1).Value = $"Период: {period.Key}";
                        worksheet.Cell(row, 1).Style.Font.Bold = true;
                        row++;

                        foreach (var warning in period.Value)
                        {
                            worksheet.Cell(row, 1).Value = warning;
                            row++;
                        }

                        row++; // Пропуск строки между периодами
                    }
                }

                // Настраиваем ширину столбцов
                worksheet.Columns().AdjustToContents();

                // Сохраняем отчет
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"Отчет_с_{startDate:MMMM_yyyy}_по_{endDate:MMMM_yyyy}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


    //Раздел нейронная сеть
        public IActionResult NetworkSettings()
        {
            ViewData["Title"] = "Настройки сети";
            return View();
        }

        // Метод для загрузки метрик и архитектуры через API

        [HttpGet]
        public async Task<IActionResult> LoadNetworkMetrics()
        {
            using var client = new HttpClient();
            try
            {
                // Запросы к Python API
                var metricsResponse = await client.GetAsync("http://127.0.0.1:5000/metrics");
                var progressResponse = await client.GetAsync("http://127.0.0.1:5000/training-progress");
                var architectureResponse = await client.GetAsync("http://127.0.0.1:5000/architecture");

                // Проверка успешности
                if (!metricsResponse.IsSuccessStatusCode || !progressResponse.IsSuccessStatusCode || !architectureResponse.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Ошибка получения данных от Python API" });
                }

                // Десериализация данных
                var metrics = await metricsResponse.Content.ReadAsStringAsync();
                var progress = await progressResponse.Content.ReadAsStringAsync();
                var architecture = await architectureResponse.Content.ReadAsStringAsync();

                return Json(new
                {
                    success = true,
                    metrics = JsonSerializer.Deserialize<object>(metrics),
                    progress = JsonSerializer.Deserialize<object>(progress),
                    architecture = JsonSerializer.Deserialize<object>(architecture)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return Json(new { success = false, message = "Ошибка подключения к Python API" });
            }
        }


        //Функция принять отклонить прогноз
        [HttpPost]
        public async Task<IActionResult> ApproveForecast(int id)
        {
            var forecast = await _context.ProductionFacts.FindAsync(id);
            if (forecast == null)
                return NotFound();

            forecast.ForecastStatus = "Принят";
            forecast.RejectionReason = null;

            await _context.SaveChangesAsync();

            return RedirectToAction("Main");
        }

        [HttpPost]
        public async Task<IActionResult> RejectForecast(int id, string reason)
        {
            var forecast = await _context.ProductionFacts.FindAsync(id);
            if (forecast == null)
                return NotFound();

            forecast.ForecastStatus = "Отклонён";
            forecast.RejectionReason = reason;
            forecast.RejectionDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Main");
        }


    }
}
