namespace RadiatorForecasting.Models
{
    public class Department
    {
        public int Id { get; set; } // Код отдела
        public string Name { get; set; } // Наименование отдела
        public int CompanyId { get; set; } // Код предприятия

        // Навигационное свойство
        public Company Company { get; set; } // Связь с предприятием
    }
}
