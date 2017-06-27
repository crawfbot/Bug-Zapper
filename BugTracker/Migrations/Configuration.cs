namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            // Seed Roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });  
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            // Seed Users
           
            //Admin
            if (!context.Users.Any(u => u.Email == "jacobcrawford1990@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jacobcrawford1990@gmail.com",
                    Email = "jacobcrawford1990@gmail.com",
                    FirstName = "Jacob",
                    LastName = "Crawford",
                    DisplayName = "Boss Hogg",
                    Role = "Admin",
                }, ".CoderFoundry$");
            }

            var userId = userManager.FindByEmail("jacobcrawford1990@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            //PM
            if (!context.Users.Any(u => u.Email == "roscoe_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "roscoe_bugzapper@mailinator.com",
                    Email = "roscoe_bugzapper@mailinator.com",
                    FirstName = "Roscoe",
                    LastName = "P Coltrane",
                    DisplayName = "Roscoe",
                    Role = "Project Manager",
                }, ".CoderFoundry$");
            }

            userId = userManager.FindByEmail("roscoe_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            //Dev
            if (!context.Users.Any(u => u.Email == "jesse_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jesse_bugzapper@mailinator.com",
                    Email = "jesse_bugzapper@mailinator.com",
                    FirstName = "Jesse",
                    LastName = "Duke",
                    DisplayName = "Uncle Jesse",
                    Role = "Developer",
                }, ".CoderFoundry$");
            }

            userId = userManager.FindByEmail("jesse_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //Submitter1
            if (!context.Users.Any(u => u.Email == "luke_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "luke_bugzapper@mailinator.com",
                    Email = "luke_bugzapper@mailinator.com",
                    FirstName = "Luke",
                    LastName = "Duke",
                    DisplayName = "Luke Duke",
                    Role = "Submitter",
                }, ".CoderFoundry$");
            }

            userId = userManager.FindByEmail("luke_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            //Submitter2
            if (!context.Users.Any(u => u.Email == "bo_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "bo_bugzapper@mailinator.com",
                    Email = "bo_bugzapper@mailinator.com",
                    FirstName = "Bo",
                    LastName = "Duke",
                    DisplayName = "Bo Duke",
                    Role = "Submitter",
                }, ".CoderFoundry$");
            }

            userId = userManager.FindByEmail("bo_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            // Guest Admin
            if (!context.Users.Any(u => u.Email == "ari_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ari_bugzapper@mailinator.com",
                    Email = "ari_bugzapper@mailinator.com",
                    FirstName = "Guest",
                    LastName = "Admin",
                    DisplayName = "Ari Admin",
                    Role = "Admin",
                }, "LearnToCode1");
            }
            userId = userManager.FindByEmail("ari_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            //Guest Project Manager
            if (!context.Users.Any(u => u.Email == "payton_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "payton_bugzapper@mailinator.com",
                    Email = "payton_bugzapper@mailinator.com",
                    FirstName = "Guest",
                    LastName = "ProjectManager",
                    DisplayName = "Payton PM",
                    Role = "Project Manager",
                }, "LearnToCode1");
            }
            userId = userManager.FindByEmail("payton_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            //Guest Developer
            if (!context.Users.Any(u => u.Email == "dakota_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "dakota_bugzapper@mailinator.com",
                    Email = "dakota_bugzapper@mailinator.com",
                    FirstName = "Guest",
                    LastName = "Developer",
                    DisplayName = "Dakota Dev",
                    Role = "Developer",
                }, "LearnToCode1");
            }
            userId = userManager.FindByEmail("dakota_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //Guest Submitter
            if (!context.Users.Any(u => u.Email == "sidney_bugzapper@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "sidney_bugzapper@mailinator.com",
                    Email = "sidney_bugzapper@mailinator.com",
                    FirstName = "Guest",
                    LastName = "Submitter",
                    DisplayName = "Sidney Submitter",
                    Role = "Submitter",
                }, "LearnToCode1");
            }
            userId = userManager.FindByEmail("sidney_bugzapper@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");


            // Ticket Status
            if (!context.TicketStatus.Any(t => t.Name == "Unassigned"))
            {
                context.TicketStatus.Add(new TicketStatus { Name = "Unassigned" });
            }
            if (!context.TicketStatus.Any(t => t.Name == "Assigned"))
            {
                context.TicketStatus.Add(new TicketStatus { Name = "Assigned" });
            }
            if (!context.TicketStatus.Any(t => t.Name == "Processing"))
            {
                context.TicketStatus.Add(new TicketStatus { Name = "Processing" });
            }
            if (!context.TicketStatus.Any(t => t.Name == "Resolved"))
            {
                context.TicketStatus.Add(new TicketStatus { Name = "Resolved" });
            }

            //context.TicketStatus.AddOrUpdate(s => s.Name,
            //    new TicketStatus() { Name = "Unassigned" },
            //    new TicketStatus() { Name = "Assigned" },
            //    new TicketStatus() { Name = "Processing" },
            //    new TicketStatus() { Name = "Resolved" }
            //    );

            //Ticket Type
            if (!context.TicketType.Any(t => t.Name == "Bug"))
            {
                context.TicketType.Add(new TicketType { Name = "Bug" });
            }
            if (!context.TicketType.Any(t => t.Name == "Enhancement"))
            {
                context.TicketType.Add(new TicketType { Name = "Enhancement" });
            }
            if (!context.TicketType.Any(t => t.Name == "Feature"))
            {
                context.TicketType.Add(new TicketType { Name = "Feature" });
            }
            if (!context.TicketType.Any(t => t.Name == "Unknown"))
            {
                context.TicketType.Add(new TicketType { Name = "Unknown" });
            }

            //context.TicketType.AddOrUpdate(t => t.Name,
            //    new TicketType() { Name = "Bug"},
            //    new TicketType() { Name = "Enhancement" },
            //    new TicketType() { Name = "Feature" },
            //    new TicketType() { Name = "Unknown" }
            //);

            //Ticket Priority
            if (!context.TicketPriority.Any(t => t.Name == "Urgent"))
            {
                context.TicketPriority.Add(new TicketPriority { Name = "Urgent" });
            }
            if (!context.TicketPriority.Any(t => t.Name == "High"))
            {
                context.TicketPriority.Add(new TicketPriority { Name = "High" });
            }
            if (!context.TicketPriority.Any(t => t.Name == "Moderate"))
            {
                context.TicketPriority.Add(new TicketPriority { Name = "Moderate" });
            }
            if (!context.TicketPriority.Any(t => t.Name == "Low"))
            {
                context.TicketPriority.Add(new TicketPriority { Name = "Low" });
            }

            //context.TicketPriority.AddOrUpdate(p => p.Name,
            //    new TicketPriority() { Name = "Urgent" },
            //    new TicketPriority() { Name = "High" },
            //    new TicketPriority() { Name = "Moderate" },
            //    new TicketPriority() { Name = "Low" }
            //);

        }
    }
}
