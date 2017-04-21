using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Budgeter.Models.CodeFirst;
using Budgeter.Helpers;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [Authorize]
    [AuthorizeHouseholdRequired]
    public class InvitedUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InvitedUsers
        public PartialViewResult _Create()
        {
            return PartialView();
        }

        // POST: InvitedUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Email,AdminRights,InviteCode")] InvitedUser invitedUser, bool HasAdminRights)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                var user = db.Users.FirstOrDefault(u => u.Id == id);
                var hId = Convert.ToInt32(User.Identity.GetHouseholdId());

                invitedUser.HouseholdId = hId;
                invitedUser.InviteCode = Membership.GeneratePassword(10, 4);
                invitedUser.InvitedBy = user.FirstName + " " + user.LastName;
                invitedUser.HasAdminRights = HasAdminRights;
                invitedUser.InvitedDate = DateTimeOffset.Now;

                db.InvitedUsers.Add(invitedUser);
                db.SaveChanges();

                var es = new EmailService();
                var msg = invitedUser.CreateJoinMessage();
                es.SendAsync(msg);

                return RedirectToAction("Details", "Households", (new { id = user.HouseholdId }));
            }

            return View(invitedUser);
        }


        // POST: InvitedUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            InvitedUser invitedUser = db.InvitedUsers.Find(id);
            db.InvitedUsers.Remove(invitedUser);
            db.SaveChanges();
            return RedirectToAction("Details", "Households");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
