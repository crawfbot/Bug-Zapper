using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using BugTracker.Helpers;
using System.IO;

namespace BugTracker
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper urh = new UserRolesHelper();
        private ProjectsHelper ph = new ProjectsHelper();
        private TicketHistoryHelper thh = new TicketHistoryHelper();

        //GET: Tickets WITH TICKET HELPER IMPLEMENTED

        public ActionResult Index()
        {
            var th = new TicketsHelper(urh, db);
            var tickets = th.GetUserTickets(User.Identity.GetUserId());

            //tickets = db.Ticket.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).ToList();

            return View(tickets);
        }


        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create ORIGINAL
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");
            return View();
        }

        // GET: Tickets/Create
        //public ActionResult Create(int id)
        //{
        //    ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");            
        //    ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");
        //    ViewBag.Developer = new MultiSelectList(urh.UsersInRole("Developer"), "Id", "DisplayName");
        //    return View(new Ticket { ProjectId = id });
        //}

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket) /*, string Developer)*/ //Took out Id,OwnerUserId,AssignedToUserId, Created, Updated
        {
            if (ModelState.IsValid)
            {
                //Code Added In Class Below
                ticket.Created = DateTimeOffset.Now;
                //ticket.Updated = DateTimeOffset.Now;
                ticket.OwnerUserId = User.Identity.GetUserId();
                //ticket.AssignedToUserId = Developer;

                db.Ticket.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var developers = urh.UsersInRole("Developer").ToList();
            var user = User.Identity.GetUserId();
            ViewBag.AssignedToUserId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedToUserId);

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket, string Developer) //Added string Developer in class
        {
            var oldtick = db.Ticket.AsNoTracking().First(x => x.Id == ticket.Id);

            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
                //ticket.AssignedToUserId = Developer; //Added in class
                db.Entry(ticket).State = EntityState.Modified;
                //db.Entry(ticket).Property("Created").IsModified = false;
                db.SaveChanges();

                var newtick = db.Ticket.AsNoTracking().First(t => t.Id == ticket.Id);

                //For Ticket History (link to TicketHelper)

                thh.NewHistory(oldtick, newtick);

                return RedirectToAction("Index");
            }
            //Added in class + string developer in ActionResult, May need to move/add to the GET. Everything that was added
            var developers = urh.UsersInRole("Developer");
            ViewBag.AssignedToUserId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedToUserId);

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Ticket.Find(id);
            db.Ticket.Remove(ticket);
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




        //POST Tickets/AttachFile
        [HttpPost]
        public ActionResult AttachFile([Bind(Include = "TicketId")] TicketAttachment ticketattachment, HttpPostedFileBase file)
        {
            Ticket ticket = db.Ticket.Find(ticketattachment.TicketId);
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    TempData["aMessage"] = "Please Upload Your file before submitting.";
                    //ModelState.AddModelError(string.Empty, "Please Upload Your file");
                }
                else if (file.ContentLength > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 3; //3 MB
                    string[] AllowedFileExtensions = new string[] { ".jpg", ".png", ".pdf", ".txt" };

                    if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ViewData["aMessage"] = "You may only upload a file of type: " + string.Join(", ", AllowedFileExtensions);
                        //ModelState.AddModelError(string.Empty, "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }

                    else if (file.ContentLength > MaxContentLength)
                    {
                        ViewData["aMessage"] = "Your file is too large. The maximum allowed size is: " + MaxContentLength + " MB";
                        ModelState.AddModelError(string.Empty, "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                    }
                    else
                    {
                        //TO:DO
                        ticketattachment.Description = Path.GetFileName(file.FileName);
                        var fileName = ticketattachment.Description;
                        ticketattachment.FilePath = "~/Media/Uploads/" + fileName;
                        file.SaveAs(Path.Combine(Server.MapPath("~/Media/Uploads/"), fileName));
                        //ModelState.Clear();

                        ticketattachment.Created = DateTimeOffset.Now;
                        ticketattachment.UserId = User.Identity.GetUserId();

                        db.TicketAttachment.Add(ticketattachment);
                        db.SaveChanges();
                        TempData["aMessage"] = "Your file was uploaded successfully";
                    }
                }
                return RedirectToAction("Details", "Tickets", new { id = ticketattachment.TicketId });
            }
            return View(ticket);
        }
    }
}
