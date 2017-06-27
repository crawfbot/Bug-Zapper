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

namespace BugTracker.Controllers
{
    public class TicketHistoryLogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketHistoryLogs
        public ActionResult Index()
        {
            var ticketHistoryLogs = db.TicketHistory.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketHistoryLogs.ToList());
        }

        // GET: TicketHistoryLogs/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistoryLog ticketHistoryLog = db.TicketHistory.Find(id);
            if (ticketHistoryLog == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistoryLog);
        }

        // GET: TicketHistoryLogs/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketHistoryLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,PropertyChanged,OldValue,NewValue,DateChanged,UserId")] TicketHistoryLog ticketHistoryLog)
        {
            if (ModelState.IsValid)
            {
                db.TicketHistory.Add(ticketHistoryLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketHistoryLog.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketHistoryLog.UserId);
            return View(ticketHistoryLog);
        }

        // GET: TicketHistoryLogs/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistoryLog ticketHistoryLog = db.TicketHistory.Find(id);
            if (ticketHistoryLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketHistoryLog.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketHistoryLog.UserId);
            return View(ticketHistoryLog);
        }

        // POST: TicketHistoryLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,PropertyChanged,OldValue,NewValue,DateChanged,UserId")] TicketHistoryLog ticketHistoryLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketHistoryLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketHistoryLog.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketHistoryLog.UserId);
            return View(ticketHistoryLog);
        }

        // GET: TicketHistoryLogs/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistoryLog ticketHistoryLog = db.TicketHistory.Find(id);
            if (ticketHistoryLog == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistoryLog);
        }

        // POST: TicketHistoryLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketHistoryLog ticketHistoryLog = db.TicketHistory.Find(id);
            db.TicketHistory.Remove(ticketHistoryLog);
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
    }
}
