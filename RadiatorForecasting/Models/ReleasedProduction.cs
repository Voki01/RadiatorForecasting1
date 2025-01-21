using System.ComponentModel.DataAnnotations;

namespace RadiatorForecasting.Models
{
    public class ReleasedProduction
    {
        public int Id { get; set; } // Первичный ключ

        public int ProductionFactId { get; set; } // Ссылка на ProductionFacts

        public ProductionFact ProductionFact { get; set; } // Навигационное свойство

        [Required]
        public int AluminumQuantity { get; set; } // Количество выпущенных алюминиевых радиаторов

        [Required]
        public int CopperQuantity { get; set; } // Количество выпущенных медных радиаторов
    }
}
