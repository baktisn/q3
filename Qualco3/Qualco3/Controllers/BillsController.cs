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
            var SetTypeDD = _context.SettlementTypes
                .OrderBy(c => c.Code)
                .Select(x => new { Id = x.ID, Value = x.Code+ " " +x.Interest+" " +x.MaxNoInstallments });
          

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
            model2.SettlementTypesEnum = _context.SettlementTypes
              .OrderBy(c => c.Code)
              .AsEnumerable();

            return View(model2);
        }


        [HttpPost]
        public async Task<JsonResult>  DDAjax1(SubmitSelected model)
        {
            var SType = await _context.SettlementTypes
                .Where(c => c.ID.Equals(model.SettlementType))
                .SingleOrDefaultAsync();

            List<int> NoInstallments = new List<int>();
            for (var m = 3; m <= SType.MaxNoInstallments; m += 3)
            {
                NoInstallments.Add(m);
            }
            
            return Json(NoInstallments);
        }


        [HttpPost]
        public IActionResult DDAjax2(SubmitSelected model)
        {

            SettlementTypes CurSettl =  _context.SettlementTypes
                .Where(c => c.ID.Equals(model.SettlementType))
                .SingleOrDefault();


                //model.BillsStr = model.BillsStr;
               // model.MaxNoOfInstallments = CurSettl.MaxNoInstallments;
                model.Interest = CurSettl.Interest;
                //model.IsAccepted = 0;
                model.DownPayment = CurSettl.DownPaymentPercentage;
            //model.TotalAmount=model.TotalAmount.ToString("N2");
            model.DownPaymentValue = model.TotalAmount * CurSettl.DownPaymentPercentage / 100;
            model.Monthly = MonthlyInstallments(model.TotalAmount, model.SettlementType, model.MaxNoOfInstallments);
            model.SettlText = "Βάση των επιλογών σας ο διακανονισμός προβλέπει προκαταβολή " + model.DownPaymentValue + ", και " + model.MaxNoOfInstallments + " μηνιαίες δόσεις ποσού "+ model.Monthly;

            return  DDAjaxBack(model);
        }

        private JsonResult DDAjaxBack(SubmitSelected model)
        {
            // do something with the model here
            // ...
            Console.WriteLine("iiiiiiiiiiii" + model.MaxNoOfInstallments.ToString());
            return Json(model);
        }



        public static decimal MonthlyInstallments(decimal amount, int type, int installments)
        {
            double interest;

            switch (type)
            {
                case 1:
                    amount = Math.Round(amount - ((amount * 10) / 100));
                    interest = 4.1 / (12 * 100);

                    return Calculate(amount, interest, installments);

                case 2:
                    amount = Math.Round(amount - ((amount * 20) / 100));
                    interest = 3.9 / (12 * 100);

                    return Calculate(amount, interest, installments);

                case 3:
                    amount = Math.Round(amount - ((amount * 30) / 100));
                    interest = 3.6 / (12 * 100);

                    return Calculate(amount, interest, installments);

                case 4:
                    amount = Math.Round(amount - ((amount * 40) / 100));
                    interest = 3.2 / (12 * 100);

                    return Calculate(amount, interest, installments);

                case 5:
                    amount = Math.Round(amount - ((amount * 50) / 100));
                    interest = 2.6 / (12 * 100);

                    return Calculate(amount, interest, installments);
            }


            decimal Calculate(decimal amount2, double interest2, int installm)
            {
                decimal Calc = (decimal)Math.Round(amount2 * ((decimal)interest2 * (decimal)Math.Pow((1 + interest2), installm)) / (((decimal)Math.Pow((1 + interest2), installm)) - 1), 2);
                Console.WriteLine(Calc);
                return Calc;
            }

            return 0;
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
