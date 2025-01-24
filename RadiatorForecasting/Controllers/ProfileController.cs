using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RadiatorForecasting.Models;


namespace RadiatorForecasting.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new UserProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user))[0],
                UserId = user.Id
            };

            return View(model);
        }
    }
}
