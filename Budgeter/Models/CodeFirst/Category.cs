using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Category
    {
        public Category()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public int? HouseholdId { get; set; }
        public string Name { get; set; }

        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}