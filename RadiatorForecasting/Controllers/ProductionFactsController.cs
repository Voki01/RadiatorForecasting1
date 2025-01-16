using Microsoft.AspNetCore.Mvc;
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
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var prediction = JsonSerializer.Deserialize<Dictionary<string, float>>(result);

                predictionResult = $"Ал: {prediction["aluminum"]:0.##}, Мед: {prediction["copper"]:0.##}";

                // Сохранение прогноза и расчёт рекомендуемых запасов
                var newFact = new ProductionFact
                {
                    AverageTemperature = averageTemperature,
                    Price = price,
                    CompetitorPrice = competitorPrice,
                    Discount = discount,
                    MonthlyForecastAluminum = (int)Math.Round(prediction["aluminum"]),
                    MonthlyForecastCopper = (int)Math.Round(prediction["copper"]),
                    CurrentAluminumStock = 100, // Пример текущего запаса
                    CurrentCopperStock = 50 // Пример текущего запаса
                };

                // Расчёт рекомендуемых запасов
                newFact.RecommendedAluminumStock = newFact.CurrentAluminumStock +
                    newFact.MonthlyForecastAluminum * ProductionFact.MaterialPerAluminumUnit;

                newFact.RecommendedCopperStock = newFact.CurrentCopperStock +
                    newFact.MonthlyForecastCopper * ProductionFact.MaterialPerCopperUnit;

                _context.ProductionFacts.Add(newFact);
                await _context.SaveChangesAsync();
            }

            TempData["PredictionResult"] = predictionResult; // Сохраняем результат прогноза
            return RedirectToAction("Index"); // Перенаправление на GET
        }
    }
}
