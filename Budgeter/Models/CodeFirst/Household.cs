using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Household
    {
        public Household()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.BudgetItems = new HashSet<BudgetItem>();
            this.InvitedUsers = new HashSet<InvitedUser>();
            this.Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual ICollection<InvitedUser> InvitedUsers { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}