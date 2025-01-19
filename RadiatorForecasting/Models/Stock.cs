namespace RadiatorForecasting.Models
{
    public class Stock
    {
        public int Id { get; set; } // Идентификатор
        public string Material { get; set; } // Материал: Алюминий или Медь
        public int CurrentStock { get; set; } // Текущий запас в килограммах
    }
}

