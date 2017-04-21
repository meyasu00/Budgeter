using Budgeter.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace Budgeter.Helpers
{
    public static class IdentityHelper
    {
        public static string GetHouseholdId(this IIdentity user)
        {
            var ClaimUser = (ClaimsIdentity)user;
            var Claim = ClaimUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId"); //go through all claims and take first to match the name we gave the claim
            if (Claim != null)
                return Claim.Value;
            else
                return null;
        }

        public static bool IsInHousehold(this IIdentity user)
        {
            var ClaimUser = (ClaimsIdentity)user;
            var hId = ClaimUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (hId.Value != null)
                return true;
            else
                return false;
            // OR return (hId != null && !string.IsNullOrWhiteSpace(hId.Value))
        }

        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
    }
}