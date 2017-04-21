using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class InvitedUser
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please provide an email address for the invited party.")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Household Invite Code")]
        public string InviteCode { get; set; }
        public string InvitedBy { get; set; }
        public DateTimeOffset InvitedDate { get; set; }
        public bool HasAdminRights { get; set; }

        public virtual Household Household { get; set; }
    }
}