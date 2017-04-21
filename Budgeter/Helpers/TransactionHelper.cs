using Budgeter.Models;
using Budgeter.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{

    public static class TransactionHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        //Account Balances
        public static decimal GetAccountBalance(this Transaction transaction)
        {
            return ModifyAccountBalance(transaction, false);
        }

        public static decimal RevertAccountBalance(this Transaction transaction)
        {
            return ModifyAccountBalance(transaction, true);
        }

        private static decimal ModifyAccountBalance(this Transaction transaction, bool Delete)
        {
            var account = db.BankAccounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);
            bool AddMoney;

            if (Delete) AddMoney = !transaction.Income;
            else AddMoney = transaction.Income;

            if (AddMoney == true)
                account.Balance += transaction.Amount;
            else
                account.Balance -= transaction.Amount;

            return account.Balance;
        }

        //Budget Balances
        public static decimal GetBudgetBalance(this Transaction transaction)
        {
            return ModifyBudgetBalance(transaction, false);
        }

        public static decimal RevertBudgetBalance(this Transaction transaction)
        {
            return ModifyBudgetBalance(transaction, true);
        }

        private static decimal ModifyBudgetBalance(this Transaction transaction, bool Delete)
        {
            var budget = db.BudgetItems.FirstOrDefault(b => b.Id == transaction.BudgetItemId);
            bool AddMoney;

            if (Delete) AddMoney = !transaction.Income;
            else AddMoney = transaction.Income;

            if (AddMoney == true)
            {
                if (budget.Income == true)
                    budget.Balance += transaction.Amount;
                else budget.Balance -= transaction.Amount;
            }
            else
            {
                if (budget.Income == true)
                    budget.Balance -= transaction.Amount;
                else budget.Balance += transaction.Amount;
            }
            return budget.Balance;
        }

        public static IdentityMessage CreateBudgetWarningMessage(this ApplicationUser user, BudgetItem budget)
        {
            var admin = user;
            var msg = new IdentityMessage();
            msg.Destination = admin.Email; //ConfigurationManager.AppSettings["ContactEmail"];
            msg.Body = "This is a notification from Budget 'er to let you know that a budget in your household - " + budget.Name + "- is getting close to its limit. You requested to be notified when the budget approached " + budget.AmountLimit + " and the balance is currently at " + budget.Balance + ". <br/><br/> To view your budget and transactions, click <a href=\"https://meyasu-budgeter.azurewebsites.net/BudgetItems/Index\">here</a>. To see your household overview, click <a href=\"https://meyasu-budgeter.azurewebsites.net/Households/Index\">here</a>. <br/><br/>Thank you for using Budgte' er!";
            msg.Subject = "Warning! Budget Limit has been exceeded";

            return msg;
        }
    }
}