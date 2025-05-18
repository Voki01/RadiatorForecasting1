using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace RadiatorForecasting.Services
{
    public class OneCServiceMock
    {
        private readonly string _filePath;

        public OneCServiceMock(IWebHostEnvironment env)
        {
            // Путь рассчитывается от корня проекта
            _filePath = Path.Combine(env.ContentRootPath, "Data", "Mock", "1c_mock_data.json");
        }

        public OneCData GetLatestParams()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("⚠️ Файл не найден: " + _filePath);
                return new OneCData();
            }

            var json = File.ReadAllText(_filePath);
            Console.WriteLine("📥 JSON из файла: " + json);

            return JsonSerializer.Deserialize<OneCData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new OneCData();
        }
    }

    public class OneCData
    {
        public int Price { get; set; }
        public int CompetitorPrice { get; set; }
        public int Discount { get; set; }
    }
}
