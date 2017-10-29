using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Db.Data;
using Db.Models;
using Microsoft.AspNetCore.Authorization;

namespace Qualco3.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SettlementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettlementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Settlements
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Settlements.Include(s => s.SettlementType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Settlements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var settlements = await _context.Settlements
                .Include(s => s.SettlementType)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (settlements == null)
            {
                return NotFound();
            }

            return View(settlements);
        }

        // GET: Settlements/Create
        public IActionResult Create()
        {
            ViewData["SettlementTypeId"] = new SelectList(_context.Set<SettlementTypes>(), "ID", "ID");
            return View();
        }

        // POST: Settlements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,RequestDate,LastName,DownPayment,Installments,Interest,IsAccepted,SettlementTypeId")] Settlements settlements)
        {
            if (ModelState.IsValid)
            {
                _context.Add(settlements);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SettlementTypeId"] = new SelectList(_context.Set<SettlementTypes>(), "ID", "ID", settlements.SettlementTypeId);
            return View(settlements);
        }

        // GET: Settlements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var settlements = await _context.Settlements.SingleOrDefaultAsync(m => m.ID == id);
            if (settlements == null)
            {
                return NotFound();
            }
            ViewData["SettlementTypeId"] = new SelectList(_context.Set<SettlementTypes>(), "ID", "ID", settlements.SettlementTypeId);
            return View(settlements);
        }

        // POST: Settlements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RequestDate,LastName,DownPayment,Installments,Interest,IsAccepted,SettlementTypeId")] Settlements settlements)
        {
            if (id != settlements.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(settlements);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettlementsExists(settlements.ID))
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
            ViewData["SettlementTypeId"] = new SelectList(_context.Set<SettlementTypes>(), "ID", "ID", settlements.SettlementTypeId);
            return View(settlements);
        }

        // GET: Settlements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var settlements = await _context.Settlements
                .Include(s => s.SettlementType)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (settlements == null)
            {
                return NotFound();
            }

            return View(settlements);
        }

        // POST: Settlements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var settlements = await _context.Settlements.SingleOrDefaultAsync(m => m.ID == id);
            _context.Settlements.Remove(settlements);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettlementsExists(int id)
        {
            return _context.Settlements.Any(e => e.ID == id);
        }
    }
}
