using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Account Name")]
        public int BankAccountId { get; set; }
        public int? CategoryId { get; set; }
        public int? BudgetItemId { get; set; }
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Transaction Date")]
        public DateTimeOffset Transacted { get; set; }
        [Display(Name = "Transaction Entered")]
        public DateTimeOffset Entered { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Income { get; set; }//how to set true to income, false to debit
        public bool Reconciled { get; set; }
        public bool IsSoftDeleted { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual Category Category { get; set; }
        public virtual BudgetItem BudgetItem { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}