using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class LogoutController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // Sign out the user
            await HttpContext.SignOutAsync();

            // Redirect to the login page or any other page after logout
            return RedirectToAction("Index", "Home");
        }
    }
}
