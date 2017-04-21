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
using Microsoft.AspNet.Identity;
using Budgeter.Helpers;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [Authorize]
    [AuthorizeHouseholdRequired]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {
            var hId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var hh = db.Households.FirstOrDefault(h => h.Id == hId);
            return View(hh);
        }

        // GET: BudgetItems/Create
        public PartialViewResult _Create()
        {
            var hId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var hh = db.Households.FirstOrDefault(h => h.Id == hId);

            ViewBag.CategoryId = new SelectList(db.Categories.Where(c => c.HouseholdId == hh.Id), "Id", "Name");
            ViewBag.WarningId = new SelectList(db.Warnings, "Id", "WarningLevel");

            return PartialView();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseholdId,CategoryId,AmountLimit,Balance,Income,WarningId,CreatorId,AllowEdits")] BudgetItem budgetItem/* bool IsIncome, bool AllowEdits*/)
        {
            if (ModelState.IsValid)
            {
                budgetItem.HouseholdId = Convert.ToInt32(User.Identity.GetHouseholdId());
                budgetItem.Balance = 0;
                budgetItem.CreatorId = User.Identity.GetUserId();
                //budgetItem.Income = IsIncome;
                //budgetItem.AllowEdits = AllowEdits;

                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();
            var hh = userId.GetHousehold();

            ViewBag.CategoryId = new SelectList(db.Categories.Where(c => c.HouseholdId == hh.Id), "Id", "Name");
            ViewBag.WarningId = new SelectList(db.Warnings, "Id", "WarningLevel");
            return RedirectToAction("Index");
        }

        // GET: BudgetItems/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            var userId = User.Identity.GetUserId();
            var hh = userId.GetHousehold();
            BudgetItem budgetItem = db.BudgetItems.Find(id);

            ViewBag.CategoryId = new SelectList(db.Categories.Where(c => c.HouseholdId == hh.Id), "Id", "Name", budgetItem.CategoryId);
            ViewBag.WarningId = new SelectList(db.Warnings, "Id", "WarningLevel", budgetItem.WarningId);
            return PartialView(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,HouseholdId,CreatorId,Name,AmountLimit,Income,WarningId,AllowEdits")] BudgetItem budgetItem, bool IsIncome, bool AllowEdits)
        {
            var userId = User.Identity.GetUserId();
            var hh = userId.GetHousehold();

            if (ModelState.IsValid)
            {
                budgetItem.Income = IsIncome;
                budgetItem.AllowEdits = AllowEdits;
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories.Where(c => c.HouseholdId == hh.Id), "Id", "Name", budgetItem.CategoryId);
            ViewBag.WarningId = new SelectList(db.Warnings, "Id", "WarningLevel");
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public PartialViewResult _Delete(int? id)
        {

            BudgetItem budgetItem = db.BudgetItems.Find(id);

            return PartialView(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            var userId = User.Identity.GetUserId();
            var hh = userId.GetHousehold();

            var transactions = db.Transactions.Where(t => t.BudgetItemId == id);
            var misc = hh.Categories.FirstOrDefault(c => c.Name == "Miscellaneous");

            foreach (var trans in transactions)
                trans.Category.Id = misc.Id;

            budgetItem.IsSoftDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: BudgetItems/_Transactions/5
        public PartialViewResult _Transactions(int? id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            return PartialView(budgetItem);
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
