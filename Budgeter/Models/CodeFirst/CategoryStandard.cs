using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class CategoryStandard
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}