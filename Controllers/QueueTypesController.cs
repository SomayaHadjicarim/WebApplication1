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
    public class QueueTypesController : Controller
    {
        private readonly WebApplication1Context _context;

        public QueueTypesController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: QueueTypes
        public IActionResult Index()
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];


            List<QueueType> queueTypes = new List<QueueType>();

            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    conn.Open(); // This line will throw an exception if the connection fails
                    var cmd = conn.CreateCommand("/queue/type/print");
                    var result = cmd.ExecuteReaderDynamic();

                    foreach (var queueType in result)
                    {
                        Console.WriteLine($"Queue Type ID: {queueType.Id}, Name: {queueType.Name}");
                        // Add more properties as needed
                        queueTypes.Add(new QueueType
                        {
                            // Assuming QueueType has properties Id, Name, Kind, etc.
                            Id = queueType.Id,
                            Name = queueType.Name,
                            Kind = queueType.Kind,
                            // Add other properties as needed
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return View(queueTypes);
        }

        // GET: QueueTypes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.QueueTypes == null)
            {
                return NotFound();
            }

            var queueType = await _context.QueueTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (queueType == null)
            {
                return NotFound();
            }

            return View(queueType);
        }

        // GET: QueueTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QueueTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOnMikrotik([Bind("Name,Kind,Rate,QueueSize,TotalQueueSize")] QueueType queueType, string ipAddress, string username, string password)
        {
            var IPADDRESS = Request.Cookies["ipaddress"];
            var USERNAME = Request.Cookies["username"];
            var PASSWORD = Request.Cookies["password"];

            // Capture the current URL
            var returnUrl = HttpContext.Request.Headers["Referer"].ToString();

            using (var conn = new MKConnection(IPADDRESS, USERNAME, PASSWORD))
            {
                try
                {
                    List<QueueType> queueTypes = new List<QueueType>();
                    conn.Open(); // This line will throw an exception if the connection fails
                    var cmd = conn.CreateCommand("/queue/type/add");

                    // Set properties for the new queue type
                    cmd.Parameters.Add("name", queueType.Name);
                    cmd.Parameters.Add("kind", queueType.Kind);

                    cmd.ExecuteNonQuery();

                    ViewBag.Message = "New Queue Type added to Mikrotik successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error adding new Queue Type to Mikrotik: {ex.Message}";
                }
            }

            // Redirect back to the previous URL
            return Redirect(returnUrl);
        }


        // GET: QueueTypes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.QueueTypes == null)
            {
                return NotFound();
            }

            var queueType = await _context.QueueTypes.FindAsync(id);
            if (queueType == null)
            {
                return NotFound();
            }
            return View(queueType);
        }

        // POST: QueueTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Kind,Rate,QueueSize,TotalQueueSize")] QueueType queueType)
        {
            if (id != queueType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(queueType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QueueTypeExists(queueType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(queueType);
        }

        // GET: QueueTypes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.QueueTypes == null)
            {
                return NotFound();
            }

            var queueType = await _context.QueueTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (queueType == null)
            {
                return NotFound();
            }

            return View(queueType);
        }

        // POST: QueueTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.QueueTypes == null)
            {
                return Problem("Entity set 'WebApplication1Context.QueueType'  is null.");
            }
            var queueType = await _context.QueueTypes.FindAsync(id);
            if (queueType != null)
            {
                _context.QueueTypes.Remove(queueType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QueueTypeExists(string id)
        {
          return (_context.QueueTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
