using Microsoft.AspNetCore.Mvc;

namespace RadiatorForecasting.Controllers
{
    public class ManagerDashboardController : Controller
    {
        public IActionResult Index()
        {
            // Отображение заглушки при первой загрузке
            return View();
        }

        public IActionResult Main()
        {
            // В будущем тут будет функционал раздела "Главная"
            return View();
        }

        public IActionResult Reports()
        {
            // В будущем тут будет функционал раздела "Формирование отчета"
            return View();
        }

        public IActionResult NetworkSettings()
        {
            // В будущем тут будет функционал раздела "Настройки сети"
            return View();
        }
    }
}
