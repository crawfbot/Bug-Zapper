using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper ph = new ProjectsHelper();

        public ActionResult Index()
        {
            return View();
        }
        // GET: Dashboard
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var tickets = new List<Ticket>();
            var attachments = new List<TicketAttachment>();
            var comments = new List<TicketComment>();

            var history = new List<TicketHistoryLog>();
            var notifications = new List<TicketNotification>();
            int projects = 0;

            if (User.IsInRole("Admin"))
            {
                tickets = db.Ticket.ToList();
                attachments = db.TicketAttachment.Take(5).ToList();
                projects = db.Project.Count();

                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComments)
                        comments.Add(comment);

                
            }
            else if (User.IsInRole("Project Manager"))
            {
                //tickets = db.Ticket.Where(t => t.AssignedToUserId == userId).ToList();

                tickets = db.Users.Find(User.Identity.GetUserId()).Project.SelectMany(p => p.Tickets).ToList();

                foreach (var ticket in tickets)
                    foreach (var attach in ticket.TicketAttachments)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComments)
                        comments.Add(comment);
                
                projects = ph.ListUserProjects(User.Identity.GetUserId()).Count();
            }
            else if (User.IsInRole("Developer") && User.IsInRole("Project Manager"))
            {
                tickets = db.Ticket.Where(t => t.Project.Users.Contains(db.Users.Find(userId))).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.TicketAttachments)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComments)
                        comments.Add(comment);
                projects = ph.ListUserProjects(User.Identity.GetUserId()).Count();
            }

            else if (User.IsInRole("Developer"))
            {
                tickets = db.Ticket.Where(t => t.AssignedToUserId == userId).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.TicketAttachments)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComments)
                        comments.Add(comment);
                projects = ph.ListUserProjects(User.Identity.GetUserId()).Count();
            }

            var model = new DashboardViewModel()
            {
                Tickets = tickets,
                Attachments = attachments,
                Comments = comments.Take(5),
                ProjectsAmt = projects
            };

            return View(model);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();

            return View(model);
        }

        public ActionResult Sent()
        {
           // ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "Bug Zapper<jacobcrawford1990@gmail.com>";
                    model.Body = "This is a message from your portfolio site. The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"]) { Subject = "Portfolio Contact Email", Body = string.Format(body, model.FromName, model.FromEmail, model.Body), IsBodyHtml = true };

                    var svc = new PersonalEmail(); await svc.SendAsync(email);

                    return RedirectToAction("Sent");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); await Task.FromResult(0); }
            }
            return View(model);
        }

        //Suggested Additional Code
        [Authorize(Roles = "Admin")]
        public ActionResult EditUsers()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditUsersInRole(string id)
        {
            var user = db.Users.Find(id);
            AdminCustomView AdminModel = new AdminCustomView();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

            return View(AdminModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditUsersInRole(AdminCustomView model, string id)
        {
            //var user = User.Identity.GetUserId();
            UserRolesHelper helper = new UserRolesHelper();
            //var currentRoles = helper.ListUserRoles(id);
            if (model.SelectedRoles == null)
            {
                model.SelectedRoles = new string[] { "" };
            }
            foreach (var role in db.Roles.Select(r => r.Name))
            {
                if (model.SelectedRoles.Contains(role))
                {
                    helper.AddUserToRole(id, role);
                }
                else
                {
                    helper.RemoveUserFromRole(id, role);
                }
            }

            return RedirectToAction("EditUsers", "Home", model);
        }
    }
}