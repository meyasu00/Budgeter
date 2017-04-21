using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class ReconBankAccount
    {
        public BankAccount Account { get; set; }
        public decimal ReconciledBalance { get; set; }
    }
}