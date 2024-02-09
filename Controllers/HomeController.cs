using Microsoft.AspNetCore.Mvc;
using MikrotikDotNet;
using System.Diagnostics;
using System.Net;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
          
            return View();
        }


        [HttpPost] // Assuming you want to handle the form submission using POST method
        public IActionResult ConnectToMikrotik(string ipAddress, string username, string password)
        {
            try
            {
                // Replace the following values with your Mikrotik credentials
                var IPADDRESS = ipAddress;
                var USERNAME = username;
                var PASSWORD = password;

                using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
                {
                    conn.Open(); // This line will throw an exception if the connection fails
                    Response.Cookies.Append("ipaddress", IPADDRESS);
                    Response.Cookies.Append("username", USERNAME);
                    Response.Cookies.Append("password", PASSWORD);

                    ViewBag.Message = "Connected to Mikrotik successfully!";
                    ViewBag.IsMikrotikConnected = true;
                    return RedirectToAction("Index", "SimpleQueues");
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to Mikrotik: {ex.Message}";
            }
            return View("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("ipaddress");
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("password");

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}