using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Squash.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Squash.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Tickets/Details/5

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if(ticket.Status.Name!="Created" && ticket.Status.Name!="Assigned" && !User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }
            var viewModel = new TicketDetailsViewModel()
            {
                Ticket = ticket,
                Priorities = db.Priorities.ToList(),
                Statuses = db.Statuses.ToList(),
                Types = db.Types.ToList(),
                AssignedUser = db.Users.FirstOrDefault(x => x.Id == ticket.AssignedUserId),
                Users = db.Users.ToList().Where(x=>x.Projects.Where(z=>z.Id==ticket.ProjectId).Any())
            };
            return View(viewModel);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter, Administrator")]
        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            return View(db.Projects.Where(x => x.Users.Any(z => z.Id == id)).ToList());
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter, Administrator")]
        public ActionResult Create(string ticketTitle, string ticketSummary, int projectId,HttpPostedFileBase fileAttachment)
        {
            if (ModelState.IsValid)
            {
                var ticket = new Ticket();
                ticket.Title = ticketTitle;
                ticket.Summary = ticketSummary;
                ticket.OwnerId = User.Identity.GetUserId();
                ticket.CreatedDate = DateTime.Now;
                ticket.ProjectId = projectId;
                ticket.StatusId = db.Statuses.FirstOrDefault(x => x.Name == "Created").Id;
                ticket.PriorityId = db.Priorities.FirstOrDefault(x => x.Name == "Medium").Id;
                ticket.TypeId = db.Types.FirstOrDefault(x => x.Name == "Other").Id;
                var newticket = db.Tickets.Add(ticket);

                if (fileAttachment != null)
                {
                    var filename = DateTime.Now.Ticks + "-" + fileAttachment.FileName;
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), filename  );
                    fileAttachment.SaveAs(path);
                    var attachment = db.TicketAttachments.Add(new TicketAttachment()
                    {
                        Description = "Initial File Attachment",
                        UploadDate = DateTime.Now,
                        UserId = User.Identity.GetUserId(),
                        TicketId = newticket.Id,
                        FilePath = "/Uploads/"+ filename
                    }); ;
                    var newAttachment = db.TicketAttachments.Add(attachment);
                    newticket.TicketAttachments.Add(newAttachment);
                }
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [Authorize(Roles ="Administrator,Project Manager,Developer")]
        public ActionResult UpdatePriority(int priorityId, int ticketId)
        {
            if (db.Priorities.Any(x => x.Id == priorityId) == false)
            {
                return RedirectToAction("Details", "Tickets", new { id=ticketId });
            }
            var ticket = db.Tickets.Find(ticketId);
            ticket.PriorityId = priorityId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id=ticketId });
        }

        [Authorize(Roles = "Administrator,Project Manager,Developer")]
        public ActionResult UpdateStatus(int statusId, int ticketId)
        {
            if (db.Statuses.Any(x => x.Id == statusId) == false)
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketId });
            }
            var ticket = db.Tickets.Find(ticketId);
            ticket.StatusId = statusId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }

        [Authorize(Roles = "Administrator,Project Manager,Developer")]
        public ActionResult UpdateType(int typeId, int ticketId)
        {
            if (db.Types.Any(x => x.Id == typeId) == false)
            {
                return RedirectToAction("Details", "Tickets", new { id=ticketId });
            }
            var ticket = db.Tickets.Find(ticketId);
            ticket.TypeId = typeId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new {id=ticketId });
        }

        [Authorize(Roles = "Administrator,Project Manager,Developer")]
        public ActionResult UpdateAssignedUser(string userId, int ticketId)
        {
            if (db.Users.Any(x => x.Id == userId) == false)
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketId });
            }
            var ticket = db.Tickets.Find(ticketId);
            ticket.AssignedUserId = userId;
            ticket.StatusId = db.Statuses.FirstOrDefault(x=>x.Name=="Assigned").Id;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }

        [Authorize(Roles = "Administrator,Project Manager,Developer")]
        public ActionResult CloseTicket(int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            var projectId = ticket.ProjectId;
            ticket.AssignedUserId = null;
            ticket.StatusId = db.Statuses.FirstOrDefault(x => x.Name == "Resolved").Id;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
