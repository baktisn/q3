using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class BillsController : Controller
    {
        private readonly MunicContext _context;

        public BillsController(MunicContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bills.ToListAsync());
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bills = await _context.Bills
                .SingleOrDefaultAsync(m => m.ID == id);
            if (bills == null)
            {
                return NotFound();
            }

            return View(bills);
        }

        // GET: Bills/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,userId,amount,description")] Bills bills)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bills);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bills);
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bills = await _context.Bills.SingleOrDefaultAsync(m => m.ID == id);
            if (bills == null)
            {
                return NotFound();
            }
            return View(bills);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,userId,amount,description")] Bills bills)
        {
            if (id != bills.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bills);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillsExists(bills.ID))
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
            return View(bills);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bills = await _context.Bills
                .SingleOrDefaultAsync(m => m.ID == id);
            if (bills == null)
            {
                return NotFound();
            }

            return View(bills);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bills = await _context.Bills.SingleOrDefaultAsync(m => m.ID == id);
            _context.Bills.Remove(bills);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillsExists(int id)
        {
            return _context.Bills.Any(e => e.ID == id);
        }
    }
}
