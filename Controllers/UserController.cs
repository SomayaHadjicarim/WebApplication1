using Microsoft.AspNetCore.Mvc;
using Microsoft.CSharp.RuntimeBinder;
using MikrotikDotNet;
using System;
using System.Dynamic;
using System.Collections.Generic;
using WebApplication1.Models;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace WebApplication1.Controllers
{

    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];

            var users = new List<User>();

            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand("/ip/hotspot/user/print ");
                    var result = cmd.ExecuteReaderDynamic();

                    foreach (dynamic user in result)
                    { 
                        var newUser = new User();

                        newUser.Name = GetValue<string>(user, "name");
                        newUser.Server = GetValue<string>(user, "server");
                        newUser.Password = GetValue<string>(user, "password");
                        newUser.Profile = GetValue<string>(user, "profile");
                        newUser.LimitUptime = TimeSpan.TryParse(GetValue<string>(user, "limit-uptime"), out var limitUptime) ? limitUptime : TimeSpan.Zero;
                        newUser.Uptime = TimeSpan.TryParse(GetValue<string>(user, "uptime"), out var uptime) ? uptime : TimeSpan.Zero;
                        newUser.BytesIn = Convert.ToInt64(GetValue<string>(user, "bytes-in"));
                        newUser.BytesOut = Convert.ToInt64(GetValue<string>(user, "bytes-out"));

                        users.Add(newUser);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error: {ex.Message}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return View(users);
        }

        public static T GetValue<T>(dynamic obj, string propertyName)
        {
            try
            {
                var propertyValue = obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
                return (T)Convert.ChangeType(propertyValue, typeof(T));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting value for property '{propertyName}': {ex.Message}");
                return default;
            }
        }




        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOnMikrotik(User user)
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];

            using var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD);
            {
                try
                {
                    conn.Open();

                    // Construct the command to add a user
                    var cmd = conn.CreateCommand("/ip/hotspot/user/add");
                    cmd.Parameters.Add("server", user.Server);
                    cmd.Parameters.Add("name", user.Name);
                    cmd.Parameters.Add("password", user.Password);
                    cmd.Parameters.Add("profile", user.Profile);

                    var uptimeString = ConvertTimeSpanToString(user.Uptime);
                    cmd.Parameters.Add("limitUptime", uptimeString);

                    // Execute the command
                    cmd.ExecuteNonQuery();

                    // Redirect to a success page or display a success message
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("input does not match any value of profile"))
                    {
                        ViewBag.ErrorMessage = "Selected profile does not exist.Please choose a valid profile.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Error: {ex.Message}";
                    }
                    return View("Create", user);
                }
            }
        }

        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOnMikrotik(string name)
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

                    var cmd = conn.CreateCommand("/ip/hotspot/user/remove");

                    // Add parameters for the user profile to be removed
                    cmd.Parameters.Add("=.id", name); // Use ".id" instead of "numbers" to specify the profile ID

                    // Execute the command
                    cmd.ExecuteNonQuery();

                    Console.WriteLine($"User '{name}' removed successfully!");
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

        public static string ConvertTimeSpanToString(TimeSpan timeSpan)
        {
            // Convert TimeSpan to a string in the format "XdYhZm" (X days, Y hours, Z minutes)
            return $"{(int)timeSpan.TotalDays}d{timeSpan.Hours}h{timeSpan.Minutes}m";
        }

    }
}


