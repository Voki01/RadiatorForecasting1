namespace RadiatorForecasting.Models
{
    public class ReleasedProductionViewModel
    {
        public int Id { get; set; } // ID ProductionFact
        public string Period { get; set; } // Период (месяц)

        public int AluminumReleased { get; set; } // Выпущенные алюминиевые радиаторы
        public int CopperReleased { get; set; } // Выпущенные медные радиаторы
    }
}
