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
using Microsoft.AspNetCore.Identity;


namespace Qualco3.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]

    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public BillsController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var userid = _userManager.GetUserId(User);
            var user = GetCurrentUserAsync();

            var applicationDbContext = _context.Bills
                .Include(b => b.ApplicationUser)
                .Include(b => b.PaymentMethods)
                .Include(b => b.Settlement)
                .Where(b => b.UserId == userid);

            //return View(await applicationDbContext.ToListAsync());
            var db = await applicationDbContext.ToListAsync();
            var model = new BillSelectionViewModel();
            foreach (var bill in db)
            {
                var editorViewModel = new SelectBillEditorViewModel()
                {
                    ID = bill.ID,
                    Bill_description = string.Format("{0}", bill.Bill_description),
                    Amount = bill.Amount,
                    DueDate = bill.DueDate,
                    Selected = false,
                    Status=bill.Status
                };
                model.Bills.Add(editorViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSelected(BillSelectionViewModel model)
        {
            // get the ids of the items selected:
            var selectedIds = model.getSelectedIds();
            // Use the ids to retrieve the records for the selected people
            // from the database:
            var userid = _userManager.GetUserId(User);
            var user = GetCurrentUserAsync();

            var applicationDbContext = _context.Bills.Include(b => b.ApplicationUser).Include(b => b.PaymentMethods).Include(b => b.Settlement).Where(b => b.UserId == userid);
            //return View(await applicationDbContext.ToListAsync());
            var db = await applicationDbContext.ToListAsync();
            decimal amount = 0;
            var selectedBill = from x in db
                               where selectedIds.Contains(x.ID)
                                 select x;
            // Process according to your requirements:
            foreach (var bill in selectedBill)
            {
                //bill.Amount = bill.Amount + amount;
                 amount += bill.Amount;
             
                System.Diagnostics.Debug.WriteLine(bill.ID + " " +bill.Bill_description  +"  " + bill.Amount.ToString() );
            }
           
            model.TotalAmount = amount;
            System.Diagnostics.Debug.WriteLine(model.TotalAmount.ToString());


            // Redirect somewhere meaningful (probably to somewhere showing 
            // the results of your processing):
            // return RedirectToAction("Index", "Settlements");


            //return View(await applicationDbContext.ToListAsync());
            var SetTypeDD = _context.SettlementTypes.OrderBy(c => c.Code).Select(x => new { Id = x.ID, Value = x.Code });
            List<int> Installments = new List<int> () {};
            //var db2 = await _context.SettlementTypes.ToListAsync();
            //var SetTypeDD = from x in db2
            //                    select x;
            //foreach (var i in SetTypeDD)
            //{
            //  Console.WriteLine(i.ID + " " + i.DownPaymentPercentage + "  " + i.Interest);
            //}
            SubmitSelected model2 = new SubmitSelected
            {
                Bills = selectedBill,
                TotalAmount = model.TotalAmount,
                Interest = 0,
                IsAccepted = 0,
                DownPayment = 0
            };

            foreach (var i in selectedIds)
            { model2.BillsStr = model2.BillsStr+ i + ",";  }
            

            model2.SettlementTypes = new SelectList(SetTypeDD, "Id", "Value");
            model2.Installments = new SelectList(Installments);

            return View(model2);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DDAjax(SubmitSelected model)
        {
            model.BillsStr = model.BillsStr;
                model.MaxNoOfInstallments = 1;
                model.Interest = 3.5M;
                model.IsAccepted = 1;
                model.DownPayment = 1.5M;
   
            return DDAjaxBack(model);
        }
        private IActionResult DDAjaxBack(SubmitSelected model)
        {
            // do something with the model here
            // ...
            Console.WriteLine("iiiiiiiiiiii" + model.MaxNoOfInstallments.ToString());
            return Json(model);
        }









        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User); 

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bills = await _context.Bills
                .Include(b => b.ApplicationUser)
                .Include(b => b.PaymentMethods)
                .Include(b => b.Settlement)
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PaymentMethodId"] = new SelectList(_context.Set<PaymentMethods>(), "ID", "ID");
            ViewData["SettlementId"] = new SelectList(_context.Set<Settlements>(), "ID", "ID");
            return View();
        }

        // POST: Bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Amount,DueDate,Bill_description,Status,UserId,PaymentMethodId,SettlementId")] Bills bills)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bills);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bills.UserId);
            ViewData["PaymentMethodId"] = new SelectList(_context.Set<PaymentMethods>(), "ID", "ID", bills.PaymentMethodId);
            ViewData["SettlementId"] = new SelectList(_context.Set<Settlements>(), "ID", "ID", bills.SettlementId);
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bills.UserId);
            ViewData["PaymentMethodId"] = new SelectList(_context.Set<PaymentMethods>(), "ID", "ID", bills.PaymentMethodId);
            ViewData["SettlementId"] = new SelectList(_context.Set<Settlements>(), "ID", "ID", bills.SettlementId);
            return View(bills);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Amount,DueDate,Bill_description,Status,UserId,PaymentMethodId,SettlementId")] Bills bills)
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bills.UserId);
            ViewData["PaymentMethodId"] = new SelectList(_context.Set<PaymentMethods>(), "ID", "ID", bills.PaymentMethodId);
            ViewData["SettlementId"] = new SelectList(_context.Set<Settlements>(), "ID", "ID", bills.SettlementId);
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
                .Include(b => b.ApplicationUser)
                .Include(b => b.PaymentMethods)
                .Include(b => b.Settlement)
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

        // GET: Bills/CreditCardPayment/5
        public async Task<IActionResult> CreditCardPayment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bills = await _context.Bills
                .Include(b => b.ApplicationUser)
                .Include(b => b.PaymentMethods)
                .Include(b => b.Settlement)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (bills == null)
            {
                return NotFound();
            }

            return View(bills);
        }

        // POST: Bills/CreditCardPayment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentConfirmed(int id)
        {
            Console.WriteLine("Bill Id   ---------  " + id.ToString());
            var cols = _context.Bills.Where(w => w.ID == id);

            foreach (var b in cols)
            {
                b.Status = 1;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool BillsExists(int id)
        {
            return _context.Bills.Any(e => e.ID == id);
        }
    }
}
