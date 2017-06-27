using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace BugTracker.Helpers
{
    public class TicketsHelper
    {
        
        private UserRolesHelper roleHelper;
        private static ApplicationDbContext context; //Added static on 3.22.17 so that GetTicketPriorityNameById, etc would work

        //simplified injection in class
        public TicketsHelper(UserRolesHelper urh, ApplicationDbContext db)
        {
            roleHelper = urh;
            context = db;
        }

        public List<Ticket> GetUserTickets(string userId)
        {
            //simplified in class
            var tickets = new List<Ticket>();
            var userRoles = roleHelper.ListUserRoles(userId).ToList();


            if (userRoles.Any(role => role == "Admin"))
            {
                tickets = context.Ticket.ToList();
                //tickets = context.Ticket.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).ToList();
            }

            else if (userRoles.Any(role => role == "Project Manager"))
            {
                tickets = context.Users.Find(userId).Project.SelectMany(p => p.Tickets).ToList();
            }

            //else if (userRoles.Any(role => role == "Developer") && userRoles.Any(role => role == "Submitter"))
            //{
            //    tickets = context.Ticket.Where(t => t.AssignedToUserId == userId || t.OwnerUserId == userId).ToList();
            //}

            else if (userRoles.Any(role => role == "Developer"))
            {
                tickets = context.Ticket.Where(t => t.AssignedToUserId == userId).ToList();
            }

            else if (userRoles.Any(role => role == "Submitter"))
            {
                tickets = context.Ticket.Where(t => t.OwnerUserId == userId).ToList();
            }

            return tickets;
        }

        public void AddTicketHistory(Ticket oldTicket, Ticket newTicket, string userId)
        {
            //Cycle over the Ticket properties and compare their values
            foreach (var prop in typeof(Ticket).GetProperties())
            {
                if (prop.GetValue(oldTicket) == null)
                    prop.SetValue(oldTicket, "", null);

                if (prop.GetValue(newTicket) == null)
                    prop.SetValue(newTicket, "", null);

                //If the value of a Ticket property has changed
                if (prop.GetValue(oldTicket).ToString() != prop.GetValue(newTicket).ToString())
                {
                    //Create a TicketHistory object to populate a new record
                    var ticketHistory = new TicketHistoryLog
                    {
                        TicketId = oldTicket.Id,
                        PropertyChanged = prop.Name.ToString(),
                        OldValue = prop.GetValue(oldTicket).ToString(),
                        NewValue = prop.GetValue(newTicket).ToString(),
                        DateChanged = DateTimeOffset.Now,
                        UserId = userId
                    };

                    //Create a new TicketHistory entry
                    context.TicketHistory.Add(ticketHistory);

                }
            } //end foreach

            context.SaveChanges();
        } // end AddTicketHistory

        public static string GetTicketPriorityNameById(int Id)
        {
            return context.TicketPriority.AsNoTracking().FirstOrDefault(tp => tp.Id == Id).Name;
        }

        public static string GetTicketStatusNameById(int Id)
        {
            return context.TicketStatus.AsNoTracking().FirstOrDefault(ts => ts.Id == Id).Name;
        }

        public static string GetTicketTypeNameById(int Id)
        {
            return context.TicketType.AsNoTracking().FirstOrDefault(tt => tt.Id == Id).Name;
        }



    }
}