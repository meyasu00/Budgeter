using Budgeter.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<ReconBankAccount> ReconBankAccounts { get; set; }
        public IEnumerable<BudgetItem> BudgetList { get; set; }
    }

    public class ManageAccountsViewModel
    {
        public IEnumerable<ReconBankAccount> ReconBankAccounts { get; set; }
    }
}