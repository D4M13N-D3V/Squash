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
using Squash.Extensions;

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
                History = db.TicketHistory.Where(x => x.TicketId == ticket.Id),
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
            var projects = db.Projects.Where(x => x.Users.Any(z => z.Id == id)).ToList();
            if (projects.Any())
            {
                return View(projects);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddComment(int ticketId, string commentBody)
        {
            if (commentBody.Length > 0)
            {
                var ticket = db.Tickets.Find(ticketId);
                var newComment = db.TicketComment.Add(new TicketComment()
                {
                    TicketId = ticketId,
                    Body = commentBody,
                    OwnerId = User.Identity.GetUserId(),
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();

                Helpers.NotificationHelpers.SendTicketNotification(ticket.Id, "Ticket '" + ticket.Title + "' has a new comment!", "Ticket has a new comment posted by " + User.Identity.GetUserFirstName() + " " + User.Identity.GetUserLastName(), db.Users.Find(User.Identity.GetUserId()));
            }
            return RedirectToAction("Details", "Tickets", new { id = ticketId });

    }
    public ActionResult UploadAttachment(int ticketId, string fileDescription, HttpPostedFileBase fileAttachment)
        {

            if (fileAttachment != null)
            {
                var ticket = db.Tickets.Find(ticketId);
                var filename = DateTime.Now.Ticks + "-" + fileAttachment.FileName;
                var path = Path.Combine(Server.MapPath("~/Uploads/"), filename);
                fileAttachment.SaveAs(path);
                var attachment = db.TicketAttachments.Add(new TicketAttachment()
                {
                    Description = fileDescription,
                    UploadDate = DateTime.Now,
                    UserId = User.Identity.GetUserId(),
                    TicketId = ticket.Id,
                    FilePath = "/Uploads/" + filename
                }); ;
                var newAttachment = db.TicketAttachments.Add(attachment);
                ticket.TicketAttachments.Add(newAttachment);

                Helpers.NotificationHelpers.SendTicketNotification(ticket.Id, "Ticket '" + ticket.Title + "' has a new attachment!", "Ticket has a new attachment uploaded by " + User.Identity.GetUserFirstName() + " " + User.Identity.GetUserLastName(), db.Users.Find(User.Identity.GetUserId()));
            }
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
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
                Helpers.NotificationHelpers.SendProjectNotification(projectId, "Ticket Submitted On " + db.Projects.Find(projectId).Name, "New ticket submitted by " + User.Identity.GetUserFirstName() + " " + User.Identity.GetUserLastName(), db.Users.Find(User.Identity.GetUserId()));
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
            var oldPriority = ticket.Priority.Name;
            ticket.PriorityId = priorityId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();

            Helpers.NotificationHelpers.SendTicketNotification(ticket.Id, "Ticket Priority Updated On '" + ticket.Title +"'", "Priority updated to " + db.Tickets.Find(ticketId).Priority.Name + "  by " + ticket.Owner.FirstName + " " + ticket.Owner.LastName, db.Users.Find(User.Identity.GetUserId()));
            Helpers.TicketHistoryHelpers.AddHistory(User.Identity.GetUserId(), ticketId, "Priority", oldPriority, db.Tickets.Find(ticketId).Priority.Name);
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
            var oldStatus = ticket.Status.Name;
            ticket.StatusId = statusId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            Helpers.NotificationHelpers.SendTicketNotification(ticket.Id, "Ticket Status Updated On '" + ticket.Title + "'", "Status updated to " + db.Tickets.Find(ticketId).Status.Name + "  by " + ticket.Owner.FirstName + " " + ticket.Owner.LastName, db.Users.Find(User.Identity.GetUserId()));
            Helpers.TicketHistoryHelpers.AddHistory(User.Identity.GetUserId(), ticketId, "Status", oldStatus, db.Tickets.Find(ticketId).Status.Name);
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
            var oldType = ticket.Type.Name;
            ticket.TypeId = typeId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            Helpers.NotificationHelpers.SendTicketNotification(ticket.Id, "Ticket Type Updated On '" + ticket.Title + "'", "Type updated to "+ db.Tickets.Find(ticketId).Type.Name + " by " + ticket.Owner.FirstName + " " + ticket.Owner.LastName, db.Users.Find(User.Identity.GetUserId()));
            Helpers.TicketHistoryHelpers.AddHistory(User.Identity.GetUserId(), ticketId, "Type", oldType, db.Tickets.Find(ticketId).Type.Name);
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
            var oldAssignedUser = "N/A";
            if (ticket.AssignedUser != null)
            {
                oldAssignedUser = ticket.AssignedUser.FirstName + " " + ticket.AssignedUser.LastName;
            }
            ticket.AssignedUserId = userId;
            ticket.StatusId = db.Statuses.FirstOrDefault(x=>x.Name=="Assigned").Id;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            Helpers.NotificationHelpers.SendTicketNotification(ticket.Id, "Ticket '" + ticket.Title + "' has been assigned!", "Ticket has been assigned to " + db.Tickets.Find(ticketId).AssignedUser.FirstName + " "+db.Tickets.Find(ticketId).AssignedUser.LastName +" by " + User.Identity.GetUserFirstName() + " " + User.Identity.GetUserLastName(), db.Users.Find(User.Identity.GetUserId()));
            Helpers.TicketHistoryHelpers.AddHistory(User.Identity.GetUserId(), ticketId, "Assigned User", oldAssignedUser, db.Users.Find(userId).FirstName +" "+ db.Users.Find(userId).LastName);
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }

        [Authorize(Roles = "Administrator,Project Manager,Developer")]
        public ActionResult CloseTicket(int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            var projectId = ticket.ProjectId;
            var oldPriority = ticket.Priority.Name;
            ticket.AssignedUserId = null;
            ticket.StatusId = db.Statuses.FirstOrDefault(x => x.Name == "Resolved").Id;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            Helpers.TicketHistoryHelpers.AddHistory(User.Identity.GetUserId(), ticketId, "Priority", oldPriority, "Resolved");
            return RedirectToAction("Index", "Home");
        }
    }
}
