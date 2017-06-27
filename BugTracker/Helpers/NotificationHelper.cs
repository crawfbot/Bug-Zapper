using BugTracker;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace BugTracker.Helpers
{
    public class NotificationHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public async Task HandleTicketNotification(int ticketId, string userName, string updatedProperties, bool reassignment)
        {
            Ticket ticket = db.Ticket.AsNoTracking().Include(t => t.Project).Include(t => t.OwnerUser).Include(t => t.TicketPriority).Include(t => t.TicketType).SingleOrDefault(t => t.Id == ticketId);

            var detail = ComposeEmailBody(ticket, userName, updatedProperties, reassignment);
            await SendNotificationEmail(ticket, userName, detail);
            InsertTicketNotification(ticket, updatedProperties, detail);
        }

        public void InsertTicketNotification(Ticket ticket, string updatedProperties, string notificationDetail)
        {
            var ticketNotification = new TicketNotification
            {
                TicketId = ticket.Id,
                Property = updatedProperties,
                NotificationDetail = notificationDetail,
                RecipientUserId = ticket.AssignedToUserId,
                Created = DateTimeOffset.Now
            };

            // Create a new TicketNotification entry
            db.TicketNotification.Add(ticketNotification);
            db.SaveChanges();
        }

        public async Task SendNotificationEmail(Ticket ticket, string userName, string notificationDetail)
        {
            // Implement the SMTP Email service in here
            try
            {
                var emailTo = db.Users.Find(ticket.AssignedToUserId).Email;
                // public MailMessage(string from, string to);
                var email = new MailMessage(WebConfigurationManager.AppSettings["emailto"], emailTo)
                {
                    Subject = "BugZapper Ticket Notification",
                    Body = notificationDetail,
                    IsBodyHtml = true
                };

                var svc = new PersonalEmail();
                await svc.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
        }

        private string ComposeEmailBody(Ticket ticket, string userName, string updatedProperties, bool reassignment)
        {
            var eMailBody = new StringBuilder();
            if (reassignment)
            {
                eMailBody.AppendFormat("One of your tickets has been updated by {0}. Details are as follows: ", userName).AppendLine();
            }
            else
            {
                eMailBody.AppendFormat("A new ticket has been assigned to you by {0}. Details are as follows: ", userName).AppendLine();
            }

            if (!string.IsNullOrEmpty(updatedProperties))
            {
                eMailBody.AppendFormat("The following property will be reflected as changed in the History Log: {0} | ", updatedProperties).AppendLine();
            }

            //eMailBody.AppendFormat("A new ticket has been assigned to you by {0}. Details are below. || ", userName).AppendLine();
            eMailBody.AppendFormat("Project: {0} | Title: {1} | Description: {2} | Submitter: {3} | Priority: {4} | Type: {5} ", ticket.Project.Name, ticket.Title, ticket.Description, ticket.OwnerUser.UserName, ticket.TicketPriority.Name, ticket.TicketType.Name);
            return eMailBody.ToString();
        }
    }
}