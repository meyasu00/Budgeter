using Budgeter.Helpers;
using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //GET: Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //POST: Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactMessage contact, string returnUrl)
        {
            var es = new EmailService();
            var msg = new IdentityMessage();
            msg.Destination = ConfigurationManager.AppSettings["ContactEmail"];
            msg.Body = "You have been sent a message from " + contact.Name + " (" + contact.Email + ") with the following contents. <br/><br/>\"" + contact.Message + "\"";
            msg.Subject = "Message received through Cachin' Cash";
            es.SendAsync(msg);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Account", null);
            }
        }

        public ActionResult GetCharts()
        {
            var hh = db.Households.Find(Convert.ToInt32(User.Identity.GetHouseholdId()));

            var accountsOverviewBar = (from account in hh.BankAccounts.Where(a => a.IsSoftDeleted != true)
                                       let income = (from transaction in account.Transactions
                                           .Where(t => t.Income == true &&
                                                  t.Transacted.DateTime.Year == 2016 &&
                                                  t.Transacted.DateTime.Month == 2)
                                                     select transaction.Amount).DefaultIfEmpty().Sum()
                                       let expense = (from trans in account.Transactions
                                            .Where(t => t.Income == false &&
                                                  t.Transacted.DateTime.Year == 2016 &&
                                                  t.Transacted.DateTime.Month == 2)
                                                      select trans.Amount).DefaultIfEmpty().Sum()
                                       select new
                                       {
                                           label = account.Name,
                                           income = income,
                                           expense = expense
                                       }).ToArray();

            var budgetsBar = (from budget in hh.BudgetItems
                              let limit = (budget.AmountLimit)
                              let balance = (budget.Balance)
                              select new
                              {
                                  label = budget.Name,
                                  limit = limit,
                                  balance = balance
                              }).ToArray();


            var expenseDonut = (from category in hh.Categories
                                let expense = (from transaction in category.Transactions
                                               where transaction.Income != true &&
                                               transaction.Transacted.DateTime.Year == 2016 &&
                                               transaction.Transacted.DateTime.Month == 2
                                               select transaction.Amount).DefaultIfEmpty().Sum()
                                where expense > 0
                                select new
                                {
                                    label = category.Name,
                                    value = expense
                                }).ToArray();

            var incomeDonut = (from category in hh.Categories
                               let income = (from transaction in category.Transactions
                                             where transaction.Income == true &&
                                             transaction.Transacted.DateTime.Year == 2016 &&
                                             transaction.Transacted.DateTime.Month == 2
                                             select transaction.Amount).DefaultIfEmpty().Sum()
                               where income > 0
                               select new
                               {
                                   label = category.Name,
                                   value = income
                               }).ToArray();

            var allData = new
            {
                accountsOverviewBar = accountsOverviewBar,
                expenseDonut = expenseDonut,
                incomeDonut = incomeDonut,
                budgetsBar = budgetsBar
            };

            return Content(JsonConvert.SerializeObject(allData), "application/json");
        }



        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}