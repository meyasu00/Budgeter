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

namespace Budgeter.Controllers
{
    [RequireHttps]
    [Authorize]
    [AuthorizeHouseholdRequired]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseholdId")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.HouseholdId = Convert.ToInt32(User.Identity.GetHouseholdId());

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Details", "Households");
            }
            return RedirectToAction("Details", "Households");
        }

        //GET: Categories/_Edit/5
        public PartialViewResult _Edit(int? id)
        {
            Category category = db.Categories.Find(id);

            return PartialView(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                var original = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == category.Id);

                if (category.Name != original.Name)
                {
                    var transactions = db.Transactions.Where(t => t.Category.Name == original.Name);
                    foreach (var trans in transactions)
                        trans.Category.Name = category.Name;

                    var budgetItems = db.BudgetItems.Where(b => b.Category.Name == original.Name);
                    foreach (var budg in budgetItems)
                        budg.Category.Name = category.Name;
                }

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Households");
            }

            return View("Details", "Households");
        }

        // GET: Categories/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            Category category = db.Categories.Find(id);

            return PartialView(category);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            var transactions = db.Transactions.Where(t => t.CategoryId == id);
            var budgetItems = db.BudgetItems.Where(b => b.CategoryId == id);
            var misc = db.Categories.FirstOrDefault(c => c.Name == "Miscellaneous").Id;

            foreach (var transaction in transactions) transaction.CategoryId = misc;
            foreach (var budg in budgetItems) budg.CategoryId = misc;

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Details", "Households");
        }
    }
}
