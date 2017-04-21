using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Budgeter.Models.CodeFirst;
using Budgeter.Helpers;
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [Authorize]
    [AuthorizeHouseholdRequired]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            Household hh = id.GetHousehold();
            var visibleAccounts = hh.BankAccounts.Where(a => a.IsSoftDeleted != true);

            var accountsList = (from account in db.BankAccounts.Include("Transactions")
                                where account.IsSoftDeleted != true && account.HouseholdId == hh.Id
                                let reconciledI = (from transaction in account.Transactions
                                                   where transaction.Reconciled == true &&
                                                   transaction.Income == true
                                                   select transaction.Amount)
                                           .DefaultIfEmpty().Sum()
                                let reconciledE = (from transaction in account.Transactions
                                                   where transaction.Reconciled == true &&
                                                   transaction.Income == false
                                                   select transaction.Amount)
                                           .DefaultIfEmpty().Sum()
                                select new ReconBankAccount
                                {
                                    Account = account,
                                    ReconciledBalance = reconciledI - reconciledE,
                                }).ToList();

            var model = new ManageAccountsViewModel
            {
                ReconBankAccounts = accountsList,
            };

            return View(model);
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public PartialViewResult _Create()
        {
            var input = TempData["formInput"];

            return PartialView(input);
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Balance")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                bankAccount.HouseholdId = Convert.ToInt32(User.Identity.GetHouseholdId());
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                var hId = Convert.ToInt32(User.Identity.GetHouseholdId());
                var hh = db.Households.Find(hId);


                Transaction originalTransaction = new Transaction()
                {
                    BankAccountId = bankAccount.Id,
                    UserId = User.Identity.GetUserId(),
                    Category = hh.Categories.FirstOrDefault((m => m.Name == "Miscellaneous")),
                    Transacted = DateTimeOffset.Now,
                    Entered = DateTimeOffset.Now,
                    Amount = bankAccount.Balance,
                    Description = "starting balance",
                    Income = true,
                    Reconciled = true,
                };

                db.Transactions.Add(originalTransaction);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            TempData["formInput"] = bankAccount;

            return RedirectToAction("Index");
        }

        // GET: BankAccounts/_Edit/5
        public PartialViewResult _Edit(int? id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            var input = TempData["formInput"];

            return PartialView(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.BankAccounts.Attach(bankAccount);
                db.Entry(bankAccount).Property("Name").IsModified = true;
                //db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData["formInput"] = bankAccount;

            return RedirectToAction("Index");
        }

        //GET: BankAccounts/_Transactions/5/
        public PartialViewResult _Transactions(int? id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            return PartialView(bankAccount);
        }

        // GET: BankAccounts/_Delete/5
        public PartialViewResult _Delete(int? id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            return PartialView(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            bankAccount.IsSoftDeleted = true;
            //db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
