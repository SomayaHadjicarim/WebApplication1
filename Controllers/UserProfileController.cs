using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using MikrotikDotNet;
using NuGet.Protocol.Core.Types;
using WebApplication1.Data;
using WebApplication1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace WebApplication1.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly WebApplication1Context _context;

        // Primary constructor
        public UserProfilesController(WebApplication1Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var IPADDRESS = Request.Cookies["ipaddress"];
                var USERNAME = Request.Cookies["username"];
                var PASSWORD = Request.Cookies["password"];

                var userProfiles = new List<UserProfile>();

                using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
                {
                    await Task.Run(() => conn.Open());
                    var cmd = conn.CreateCommand("/ip/hotspot/user/profile/print");
                    var result = await Task.Run(() => cmd.ExecuteReaderDynamic());

                    foreach (var profile in result)
                    {


                        try
                        {
                            string rateLimit = profile.RateLimit;
                            userProfiles.Add(new UserProfile
                            {
                                Name = profile.Name,
                                SharedUsers = Convert.ToInt32(profile.SharedUsers),
                                RateLimit = FormatLimit(rateLimit.Split('/')[0]), 
                                SessionTimeout = profile.SessionTimeout,
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error accessing RateLimit: {ex.Message}");
                        }
                    }
                }

                return View(userProfiles);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                Console.WriteLine($"Error: {ex.Message}");
                return View(new List<UserProfile>());
            }
        }


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
        public IActionResult CreateOnMikrotik([Bind("Name, SharedUsers, RateLimit, SessionTimeout, ParentQueue")] UserProfile userProfile)
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

                    var cmd = conn.CreateCommand("/ip/hotspot/user/profile/add");

                    // Add parameters for the new user profile
                    cmd.Parameters.Add("name", userProfile.Name);
                    cmd.Parameters.Add("shared-users", userProfile.SharedUsers.ToString());
                    cmd.Parameters.Add("rate-limit", userProfile.RateLimit.ToString());
                    cmd.Parameters.Add("session-timeout", userProfile.SessionTimeout.ToString());

                    // Execute the command
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("User profile added successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return Redirect(returnUrl);
        }

        public IActionResult Edit()
        {
            // You can implement the logic to retrieve the user profile details for editing
            // and pass them to the view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOnMikrotik(string name, string RateLimit)
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

                    var cmd = conn.CreateCommand("/ip/hotspot/user/profile/set");

                    // Add parameters for the user profile to be edited
                    cmd.Parameters.Add("numbers", name);
                    cmd.Parameters.Add("rate-limit", RateLimit);

                    // Execute the command
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("User profile edited successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return Redirect(returnUrl);
        }

        public IActionResult Remove()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveOnMikrotik(string name)
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];
            var returnUrl = HttpContext.Request.Headers.Referer.ToString();

            try
            {
                using var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD);
                {
                    conn.Open();

                    var cmd = conn.CreateCommand("/ip/hotspot/user/profile/remove");

                    // Add parameters for the user profile to be removed
                    cmd.Parameters.Add("=.id", name); // Use ".id" instead of "numbers" to specify the profile ID

                    // Execute the command
                    cmd.ExecuteNonQuery();

                    Console.WriteLine($"User profile '{name}' removed successfully!");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                Console.WriteLine($"Error: {ex.Message}");
                // Log the full exception details
                Console.WriteLine(ex.ToString());
            }

            return Redirect(returnUrl);
        }
    }
}