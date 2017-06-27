using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker
{
    //[RequireHttps] //Will Not Work At Home
    [Authorize(Roles = "Admin, Project Manager")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper ph = new ProjectsHelper();
        private UserRolesHelper urh = new UserRolesHelper();

        // GET: Projects
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (!User.IsInRole("Admin"))
            {
                return View(user.Project.OrderByDescending(p => p.Id).ToList());
            }
            return View(db.Project.OrderByDescending(p => p.Id).ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int id)
        {
            var ph = new ProjectsHelper();

            var usersAssigned = ph.UsersOnProject(id);
            var selectedUsers = new List<string>();
            Project projects = db.Project.Find(id);
            foreach (var user in usersAssigned)
            {
                selectedUsers.Add(user.Id);
            }
            ViewBag.AssignedUsers = new MultiSelectList(db.Users, "Id", "FullName", selectedUsers);

            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Details/5
        [HttpPost]
        public ActionResult Details(int id, List<string> AssignedUsers)
        {
            var ph = new ProjectsHelper();
            if (AssignedUsers == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            foreach (var user in db.Users.ToList())
            {
                ph.RemoveUserFromProject(user.Id, id);
            }
            foreach (var user in AssignedUsers)
            {
                ph.AddUserToProject(user, id);
            }


            var users = ph.UsersOnProject(id);
            var selectedUsers = new List<string>();
            foreach (var user in users)
            {
                selectedUsers.Add(user.Id);
            }
            Project project = db.Project.Find(id);


            ViewBag.AssignedUsers = new MultiSelectList(db.Users, "Id", "FullName", selectedUsers);

            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }


        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTimeOffset.Now;
                //project.Updated = DateTimeOffset.Now;

                db.Project.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                
                project.Updated = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;

                db.Entry(project).Property("Created").IsModified = false;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Project.Find(id);
            db.Project.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //GET :: Add User
        public ActionResult AddUserToProject(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(projectId);

            if (project == null)
            {
                return HttpNotFound();
            }

            // user ids of those currently on the project
            var assignedUserIds = ph.UserIdsOnProject((int)projectId);

            ViewBag.Users = new MultiSelectList(db.Users, "Id", "FullName", assignedUserIds);
            return View(project);
        }

        //POST :: Add User
        [HttpPost]
        public ActionResult AddUserToProject(int projectId, List<string> Users)
        {
            if (Users == null)

                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                foreach (var userId in Users)
                {
                    ph.AddUserToProject(userId, projectId);

                }

                return RedirectToAction("Index");
            }

            ViewBag.Users = new MultiSelectList(db.Users, "Id", "FullName", Users); //return to where I left off
            return View();
        }

        //GET :: Remove User
        public ActionResult RemoveUserFromProject(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(projectId);

            if (project == null)
            {
                return HttpNotFound();
            }

            int pId = (int)projectId;
            //users currently on project
            var userList = ph.UsersOnProject(pId);

            // user ids of those currently on the project
            var userIdList = ph.UserIdsOnProject(pId);

            ViewBag.AssignedUsers = new MultiSelectList(userList, "Id", "FullName", userIdList);
            return View(project);
        }

        //POST :: Remove User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUserFromProject(int projectId, List<string> AssignedUsers)
        {
            if (AssignedUsers == null)

                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                foreach (var userId in AssignedUsers)
                {
                    ph.RemoveUserFromProject(userId, projectId);

                }

                return RedirectToAction("Index");
            }

            var userList = ph.UsersOnProject(projectId);

            ViewBag.AssignedUsers = new MultiSelectList(userList, "Id", "FullName", AssignedUsers); //return to where I left off
            return View();
        }

    }

}
