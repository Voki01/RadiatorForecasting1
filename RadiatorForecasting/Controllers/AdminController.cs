using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace RadiatorForecasting.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Users()
        {
            var users = _userManager.Users.Select(u => new
            {
                u.Id,
                u.Email,
                u.UserName
            }).ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        user.Email = model.Email;
                        user.UserName = model.Email; // Обновляем логин, если Email изменен
                    }

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                        if (!result.Succeeded)
                        {
                            return BadRequest(new { Message = "Ошибка при обновлении пароля." });
                        }
                    }

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        return Ok(new { Message = "Пользователь успешно обновлен." });
                    }
                }
            }

            return BadRequest(new { Message = "Ошибка при обновлении пользователя." });
        }
    }

    public class UpdateUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
