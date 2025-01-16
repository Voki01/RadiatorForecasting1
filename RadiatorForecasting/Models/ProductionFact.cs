namespace RadiatorForecasting.Models
{
    using System.ComponentModel.DataAnnotations;
    public class ProductionFact
    {
        [Key]
        public int Id { get; set; }

        [Range(-50, 50, ErrorMessage = "Температура должна быть от -50 до 50")]
        public int AverageTemperature { get; set; } // Средняя температура

        public int Price { get; set; } // Цена
        public int CompetitorPrice { get; set; } // Цена конкурентов

        [Range(0, 100, ErrorMessage = "Скидка должна быть от 0 до 100")]
        public int Discount { get; set; } // Скидка

        public int AluminumRadiatorsProduced { get; set; } // Алюминиевые радиаторы
        public int CopperRadiatorsProduced { get; set; } // Медные радиаторы

    }
}
