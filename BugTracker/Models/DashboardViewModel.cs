using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<TicketAttachment> Attachments { get; set; }
        public IEnumerable<TicketComment> Comments { get; set; }

        public IEnumerable<TicketNotification> Notifications { get; set; }

        public IEnumerable<TicketHistoryLog> History { get; set; }
        
        public IEnumerable<Project> Projects { get; set; }

        public int ProjectsAmt { get; set; }
    }
}