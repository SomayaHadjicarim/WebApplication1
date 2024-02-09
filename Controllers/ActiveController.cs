using System;
using System.Collections.Generic;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using MikrotikDotNet;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ActiveController : Controller
    {
        public IActionResult Index()
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];

            List<Active> activeUsers = new List<Active>();


            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand("/ip/hotspot/active/print");
                    var result = cmd.ExecuteReaderDynamic();

                    foreach (var activeUser in result)
                    {
                        System.Diagnostics.Debug.WriteLine($"Active User Result: {activeUser}");
                        activeUsers.Add(new Active
                        {
                            User = activeUser.User,
                            Server = activeUser.Server,
                            IPAddress = activeUser.Address,
                            MacAddress = activeUser.MacAddress,
                            SessionTime = activeUser.Uptime,
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return View(activeUsers);
        }

    }
}
