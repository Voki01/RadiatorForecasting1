using Microsoft.AspNetCore.Mvc;
using RadiatorForecasting.Data; // Пространство контекста
using RadiatorForecasting.Models; // Пространство имен модели
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace RadiatorForecasting.Controllers
{
    public class ProductionFactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;


        // Конструктор с инъекцией зависимостей
        public ProductionFactsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }


        public IActionResult Index()
        {
            var productionFacts = _context.ProductionFacts.ToList();

            // Передаем объект для ввода параметров
            ViewBag.PredictionResult = null; // Сюда будем помещать результат прогноза
            return View(productionFacts);
        }

        // Новый метод для отображения формы
        [HttpGet]
        public IActionResult AddForecastParams()
        {
            return View();
        }

        // Обработка отправленных данных
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
            }

            // Передаём результат и введённые значения обратно в представление
            ViewBag.PredictionResult = predictionResult;
            ViewBag.InputValues = new
            {
                averageTemperature,
                price,
                competitorPrice,
                discount
            };

            var productionFacts = _context.ProductionFacts.ToList();
            return View(productionFacts);
        }
    }
}
