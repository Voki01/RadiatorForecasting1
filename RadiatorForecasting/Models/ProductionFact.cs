namespace RadiatorForecasting.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductionFact
    {
        [Key]
        public int Id { get; set; }
        public string ForecastMonth { get; set; } // Столбец для отображения периода (месяц)

        [Range(-50, 50, ErrorMessage = "Температура должна быть от -50 до 50")]
        public int AverageTemperature { get; set; } // Средняя температура

        public int Price { get; set; } // Цена
        public int CompetitorPrice { get; set; } // Цена конкурентов

        [Range(0, 100, ErrorMessage = "Скидка должна быть от 0 до 100")]
        public int Discount { get; set; } // Скидка

        public int AluminumRadiatorsProduced { get; set; } // Алюминиевые радиаторы
        public int CopperRadiatorsProduced { get; set; } // Медные радиаторы

        // Поля для прогноза
        public int? MonthlyForecastAluminum { get; set; } // Прогноз алюминиевых радиаторов
        public int? MonthlyForecastCopper { get; set; } // Прогноз медных радиаторов

        // Поля для запасов
        public int? CurrentAluminumStock { get; set; } // Текущие запасы алюминия (в кг)
        public int? RecommendedAluminumStock { get; set; } // Рекомендуемые запасы алюминия (в кг)
        public int? CurrentCopperStock { get; set; } // Текущие запасы меди (в кг)
        public int? RecommendedCopperStock { get; set; } // Рекомендуемые запасы меди (в кг)

        // Коэффициенты расхода материалов
        public const int MaterialPerAluminumUnit = 3; // 3 кг алюминия на 1 радиатор
        public const int MaterialPerCopperUnit = 5; // 5 кг меди на 1 радиатор

    }
}
