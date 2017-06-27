using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            TicketComments = new HashSet<TicketComment>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketHistoryLogs = new HashSet<TicketHistoryLog>();
            TicketNotifications = new HashSet<TicketNotification>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; } 

        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }

        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistoryLog> TicketHistoryLogs { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

    }

    
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public string UserId { get; set; } 
                        
        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }

    public class TicketComment
    {
        public int Id { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }             

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class TicketHistoryLog
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string PropertyChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset? DateChanged { get; set; }
        public string UserId { get; set; }             
        
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
   
    }

    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    
    }

    public class TicketNotification
    {
        public int Id { get; set; }

        [Display(Name = "Ticket")]
        public int TicketId { get; set; }

        public string Property { get; set; }

        [Display(Name = "Message")]
        public string NotificationDetail { get; set; }

        [Display(Name = "Recipient")]
        public string RecipientUserId { get; set; }

        [Display(Name = "Sent")]
        public DateTimeOffset Created { get; set; }

        public bool IsRead { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser RecipientUser { get; set; }

        public string CreatedFormatted { get { return Created.ToString("ddd, dd MMMM yyyy h:mm tt"); } }
    }

    //public class TicketNotification
    //{
    //    public int Id { get; set; }
    //    public int TicketId { get; set; }
    //    public string UserId { get; set; }

    //    [AllowHtml]
    //    public string Detail { get; set; }

    //    public bool IsRead { get; set; }

    //    public DateTimeOffset? GeneratedDt { get; set; }

    //    public virtual Ticket Ticket { get; set; }
    //}
}