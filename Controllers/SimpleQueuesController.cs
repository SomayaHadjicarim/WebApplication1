using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MikrotikDotNet;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SimpleQueuesController : Controller
    {
        private readonly WebApplication1Context _context;
        public SimpleQueuesController(WebApplication1Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];

            var simpleQueues = new List<SimpleQueue>();

            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand("/queue/simple/print");
                    var result = cmd.ExecuteReaderDynamic();

                    foreach (var simpleQueue in result)
                    {
                        simpleQueues.Add(new SimpleQueue
                        {
                            Name = simpleQueue.Name,
                            Target = simpleQueue.Target,
                            UploadMaxLimit = FormatLimit(simpleQueue.MaxLimit.Split('/')[0]),
                            DownloadMaxLimit = FormatLimit(simpleQueue.MaxLimit.Split('/')[1]),
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return View(simpleQueues);
        }

        // Function to format the limit
        public static string FormatLimit(string limit)
        {
            if (int.TryParse(limit, out int limitValue))
            {
                if (limitValue >= 1000000)
                {
                    return $"{limitValue / 1000000}M";
                }
                else if (limitValue >= 1000)
                {
                    return $"{limitValue / 1000}K";
                }
                else
                {
                    return limit;
                }
            }
            else
            {
                return limit;
            }
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOnMikrotik([Bind("Name,Target,UploadMaxLimit,DownloadMaxLimit")] SimpleQueue simpleQueue)
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];
            var returnUrl = HttpContext.Request.Headers.Referer.ToString();

            using var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD);
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand("/queue/simple/add");

                    cmd.Parameters.Add("name", simpleQueue.Name);
                    cmd.Parameters.Add("target", simpleQueue.Target);
                    cmd.Parameters.Add("max-limit", $"{simpleQueue.UploadMaxLimit}/{simpleQueue.DownloadMaxLimit}");

                    cmd.ExecuteNonQuery();

                    ViewBag.Message = "New Simple Queue added to Mikrotik successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error adding new Simple Queue to Mikrotik: {ex.Message}";
                }
            }

            return Redirect(returnUrl);
        }
        public IActionResult EditOnMikrotik(String name, string uploadMaxLimit, string downloadMaxLimit)
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];
            var returnUrl = HttpContext.Request.Headers.Referer.ToString();

            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand("/queue/simple/set");

                    // Assuming 'row' is used to identify the queue number or some identifier
                    cmd.Parameters.Add("numbers", name);
                    cmd.Parameters.Add("max-limit", $"{uploadMaxLimit}/{downloadMaxLimit}");

                    // Print the command to the console
                    Console.WriteLine("Command to be executed: " + cmd.CommandText);

                    cmd.ExecuteNonQuery();

                    ViewBag.Message = "Simple Queue edited on Mikrotik successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error editing Simple Queue on Mikrotik: {ex.Message}";
                }
            }

            return Redirect(returnUrl);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveOnMikrotik(string name)
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];
            var returnUrl = HttpContext.Request.Headers.Referer.ToString();

            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand("/queue/simple/remove");

                    // Assuming 'name' is used to identify the queue name
                    cmd.Parameters.Add("numbers", name);

                    // Print the command to the console
                    Console.WriteLine("Command to be executed: " + cmd.CommandText);

                    cmd.ExecuteNonQuery();

                    ViewBag.Message = "Simple Queue removed from Mikrotik successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error removing Simple Queue from Mikrotik: {ex.Message}";
                }
            }

            return Redirect(returnUrl);
        }

    }
}


