using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class TicketHistoryHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public void NewHistory(Ticket oldtick, Ticket newtick)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();

            foreach(var prop in typeof(Ticket).GetProperties())
            {
                if(prop.Name == "AssignedToUser")
                {
                    break;
                }

                try
                {
                    if (prop.GetValue(oldtick) == null)
                        prop.SetValue(oldtick, "", null);
                    if (prop.GetValue(newtick) == null)
                        prop.SetValue(newtick, "", null);
                }
                
                finally
                {
                    if (prop.GetValue(oldtick).ToString() != prop.GetValue(newtick).ToString())
                    {
                        var ticketHistory = new TicketHistoryLog
                        {
                            TicketId = oldtick.Id,
                            PropertyChanged = prop.Name,
                            OldValue = prop.GetValue(oldtick).ToString(),
                            NewValue = prop.GetValue(newtick).ToString(),
                            DateChanged = DateTime.Now,
                            UserId = userId
                        };
                        db.TicketHistory.Add(ticketHistory);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static string SorterOld(int Id)
        {
            var tickhist = db.TicketHistory.Find(Id);
            if (tickhist.OldValue.ToString() == "")
            {
                return "";
            }
            switch (tickhist.PropertyChanged)
            {

                case "ProjectId":
                    return db.Project.Find(Convert.ToInt32(tickhist.OldValue)).Name;
                    break;
                case "TicketTypeId":
                    return db.TicketType.Find(Convert.ToInt32(tickhist.OldValue)).Name;
                    break;
                case "TicketPriorityId":
                    return db.TicketPriority.Find(Convert.ToInt32(tickhist.OldValue)).Name;
                    break;
                case "StatusId":
                    return db.TicketStatus.Find(Convert.ToInt32(tickhist.OldValue)).Name;
                    break;
                case "OwnerUserId":
                    return db.Users.Find(tickhist.OldValue).FullName;
                    break;
                case "AssignedToUserId":
                    return db.Users.Find(tickhist.OldValue).FullName;
                    break;
                default:
                    return tickhist.OldValue;

            }
        }
        public static string SorterNew(int Id)
        {
            var tickhist = db.TicketHistory.Find(Id);
            if (tickhist.NewValue.ToString() == "")
            {
                return "";
            }
            switch (tickhist.PropertyChanged)
            {
                case "ProjectId":
                    return db.Project.Find(Convert.ToInt32(tickhist.NewValue)).Name;
                    break;
                case "TicketTypeId":
                    return db.TicketType.Find(Convert.ToInt32(tickhist.NewValue)).Name;
                    break;
                case "TicketPriorityId":
                    return db.TicketPriority.Find(Convert.ToInt32(tickhist.NewValue)).Name;
                    break;
                case "StatusId":
                    return db.TicketStatus.Find(Convert.ToInt32(tickhist.NewValue)).Name;
                    break;
                case "OwnerUserId":
                    return db.Users.Find(tickhist.NewValue).FullName;
                    break;
                case "AssignedToUserId":
                    return db.Users.Find(tickhist.NewValue).FullName;
                    break;
                default:
                    return tickhist.NewValue;

            }
        }

        //public static string ManageData(TicketHistoryLog item, int index)
        //{
        //    var data = "";

        //    switch (item.PropertyChanged)
        //    {
        //        case "ProjectId":
        //            data = index == 0 ? ProjectsHelper.GetProjectNameById(Convert.ToInt32(item.OldValue)) : ProjectsHelper.GetProjectNameById(Convert.ToInt32(item.NewValue));
        //            break;

        //        case "AssignedToUserId":
        //            data = index == 0 ? UserHelper.GetDisplayNameFromId(item.OldValue) : UserHelper.GetDisplayNameFromId(item.NewValue);
        //            break;

        //        case "RoleId":
        //            data = index == 0 ? RoleHelper.GetRoleNameById(item.OldValue) : RoleHelper.GetRoleNameById(item.NewValue);
        //            break;

        //        case "TicketPriorityId":
        //            data = index == 0 ? TicketsHelper.GetTicketPriorityNameById(Convert.ToInt32(item.OldValue)) : TicketsHelper.GetTicketPriorityNameById(Convert.ToInt32(item.NewValue));
        //            break;

        //        case "TicketStatusId":
        //            data = index == 0 ? TicketsHelper.GetTicketStatusNameById(Convert.ToInt32(item.OldValue)) : TicketsHelper.GetTicketStatusNameById(Convert.ToInt32(item.NewValue));
        //            break;

        //        case "TicketTypeId":
        //            data = index == 0 ? TicketsHelper.GetTicketTypeNameById(Convert.ToInt32(item.OldValue)) : TicketsHelper.GetTicketTypeNameById(Convert.ToInt32(item.NewValue));
        //            break;

        //        case "OwnerUserId":
        //            data = index == 0 ? UserHelper.GetDisplayNameFromId(item.OldValue) : UserHelper.GetDisplayNameFromId(item.NewValue);
        //            break;

        //        default:
        //            data = index == 0 ? item.OldValue : item.NewValue;
        //            break;

        //    }

        //    return data;

        //}

    }
}