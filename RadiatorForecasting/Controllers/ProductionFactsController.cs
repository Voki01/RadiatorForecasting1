﻿using Microsoft.AspNetCore.Mvc;
using RadiatorForecasting.Data;
using RadiatorForecasting.Models;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace RadiatorForecasting.Controllers
{
    public class ProductionFactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductionFactsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            var productionFacts = _context.ProductionFacts.ToList();
            ViewBag.PredictionResult = null;
            return View(productionFacts);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int averageTemperature, int price, int competitorPrice, int discount)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "http://127.0.0.1:5000/predict";

            var payload = new
            {
                averageTemperature,
                price,
                competitorPrice,
                discount
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            string predictionResult = "Ошибка при выполнении прогноза.";
            int aluminumForecast = 0;
            int copperForecast = 0;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var prediction = JsonSerializer.Deserialize<Dictionary<string, float>>(result);

                aluminumForecast = (int)Math.Round(prediction["aluminum"]);
                copperForecast = (int)Math.Round(prediction["copper"]);

                predictionResult = $"Ал: {aluminumForecast:0.##}, Мед: {copperForecast:0.##}";
            }

            // Получение текущих запасов из базы данных
            var aluminumStock = _context.Stocks.FirstOrDefault(s => s.Material == "Алюминий")?.CurrentStock ?? 0;
            var copperStock = _context.Stocks.FirstOrDefault(s => s.Material == "Медь")?.CurrentStock ?? 0;

            // Расчёт рекомендуемых запасов
            int recommendedAluminumStock = aluminumForecast * ProductionFact.MaterialPerAluminumUnit;
            int recommendedCopperStock = copperForecast * ProductionFact.MaterialPerCopperUnit;

            // Вычисление нехватки запасов
            int aluminumShortage = Math.Max(0, recommendedAluminumStock - aluminumStock);
            int copperShortage = Math.Max(0, recommendedCopperStock - copperStock);

            // Вычисление, на сколько радиаторов хватает текущих запасов
            int aluminumRadiatorsPossible = aluminumStock / ProductionFact.MaterialPerAluminumUnit;
            int copperRadiatorsPossible = copperStock / ProductionFact.MaterialPerCopperUnit;

            // Формирование предупреждения
            string warningMessage = "";
            if (aluminumShortage > 0 || copperShortage > 0)
            {
                warningMessage += "Предупреждение:\n";
                if (aluminumShortage > 0)
                {
                    warningMessage += $"Не хватает {aluminumShortage} кг алюминия для производства {aluminumForecast} радиаторов.\n";
                }
                if (copperShortage > 0)
                {
                    warningMessage += $"Не хватает {copperShortage} кг меди для производства {copperForecast} радиаторов.\n";
                }
                warningMessage += "Рекомендуется пополнить запасы.\n\n";

                warningMessage += $"Текущих запасов алюминия хватит на производство {aluminumRadiatorsPossible} радиаторов.\n";
                warningMessage += $"Текущих запасов меди хватит на производство {copperRadiatorsPossible} радиаторов.\n";
            }

            // Вычисление месяца прогноза
            var nextMonth = DateTime.Now.AddMonths(1).ToString("MMMM yyyy");

            // Добавление записи в таблицу "Факты выпуска"
            var newFact = new ProductionFact
            {
                ForecastMonth = nextMonth, // Устанавливаем период прогноза
                AverageTemperature = averageTemperature,
                Price = price,
                CompetitorPrice = competitorPrice,
                Discount = discount,
                MonthlyForecastAluminum = aluminumForecast,
                MonthlyForecastCopper = copperForecast,
                CurrentAluminumStock = aluminumStock,
                CurrentCopperStock = copperStock,
                RecommendedAluminumStock = recommendedAluminumStock,
                RecommendedCopperStock = recommendedCopperStock
            };

            _context.ProductionFacts.Add(newFact);
            await _context.SaveChangesAsync();

            TempData["PredictionResult"] = predictionResult;
            TempData["WarningMessage"] = warningMessage;

            return RedirectToAction("Index");
        }


    }
}
