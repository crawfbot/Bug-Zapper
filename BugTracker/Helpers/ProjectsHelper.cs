using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTracker.Models;

namespace BugTracker.Helpers
{
    public class ProjectsHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        //Added in class, Friday 3.17.17. Can build to mirror the TicketsHelper
        //public List<Project> MyProjects(string userId)
        //{

        //}
        

        //Determines if a user is in a specific project
        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Project.Find(projectId);
            var isAssigned = project.Users.Any(u => u.Id == userId);
            return (isAssigned);
        }

        //Should return all projects assigned to a specific user
        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Project.ToList();
            return (projects);
        }

        // Assign a user to a project, if not already assigned
        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Project.Find(projectId);
                var newUser = db.Users.Find(userId);
                proj.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        //Remove a user from a project, if already assigned
        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Project.Find(projectId);
                var delUser = db.Users.Find(userId);
                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified; // just saves this obj instance.
                db.SaveChanges();
            }
        }

        //List all users assigned to a project
        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Project.Find(projectId).Users;
        }

        //List all users not on a project
        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Project.All(p => p.Id != projectId)).ToList();
        }

        public List<string> UserIdsOnProject(int projectId)
        {
            var tempList = new List<string>();
            var tempUsers = db.Project.Find(projectId).Users;
            foreach (var user in tempUsers)
            {
                tempList.Add(user.Id);
            }
            return tempList;
        }

        public static string GetProjectNameById(int Id)
        {
            return db.Project.AsNoTracking().FirstOrDefault(pn => pn.Id == Id).Name;
        }
    }
}