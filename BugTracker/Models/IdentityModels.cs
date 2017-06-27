using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string DisplayName { get; set; }
        public string Role { get; set; }
        //public string MediaURL { get; set; }
        //public string TeamId { get; set; }

        public ApplicationUser()
        {
            this.TicketNotification = new HashSet<TicketNotification>();
            this.TicketHistory = new HashSet<TicketHistoryLog>();
            this.TicketComment = new HashSet<TicketComment>();
            this.TicketAttachment = new HashSet<TicketAttachment>();

            this.Project = new HashSet<Project>();

            //was on Jason's. May not need here. Double check.
            //this.OwnerUser = new HashSet<Ticket>();
            //this.AssignedToUser = new HashSet<Ticket>();
        }

        //Can pluralize the text in white but will have to change throughout all controllers and helpers, etc
        public virtual ICollection<TicketNotification> TicketNotification { get; set; } 
        public virtual ICollection<TicketHistoryLog> TicketHistory { get; set; }
        public virtual ICollection<TicketComment> TicketComment { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachment { get; set; }

        public virtual ICollection<Project> Project { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //Can pluralize the text in white but will have to change throughout all controllers and helpers, etc
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<TicketAttachment> TicketAttachment { get; set; }
        public DbSet<TicketComment> TicketComment { get; set; }
        public DbSet<TicketHistoryLog> TicketHistory { get; set; }
        public DbSet<TicketNotification> TicketNotification { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
        public DbSet<TicketStatus> TicketStatus { get; set; }
        public DbSet<TicketPriority> TicketPriority { get; set; }
        public DbSet<Project> Project { get; set; }
    }
}