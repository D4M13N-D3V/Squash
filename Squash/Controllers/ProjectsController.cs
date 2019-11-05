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
namespace Squash.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult AddToProject(string userId, int projectId)
        {
            Helpers.ProjectHelpers.AddUserToProject(userId, projectId);
            var projectName = db.Projects.Find(projectId).Name;
            var user = db.Users.Find(userId);
            var fullName = user.FirstName + " " + user.LastName;
            Helpers.NotificationHelpers.SendProjectNotification(projectId, "Project Member Added!", "'" + fullName + "' was added to '"+projectName+"'!", db.Users.Find(User.Identity.GetUserId()));
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult RemoveFromProject(string userId, int projectId)
        {
            Helpers.ProjectHelpers.RemoveUserFromProject(userId, projectId);
            var projectName = db.Projects.Find(projectId).Name;
            var user = db.Users.Find(userId);
            var fullName = user.FirstName + " " + user.LastName;
            Helpers.NotificationHelpers.SendProjectNotification(projectId, "Project Member Removed!", "'" + fullName + "' was removed from '" + projectName + "'!", db.Users.Find(User.Identity.GetUserId()));
            foreach(var ticket in user.AssignedTickets)
            {
                ticket.AssignedUserId = null;
                db.Entry(ticket).State = EntityState.Modified;
            }
            db.SaveChanges();    
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [Authorize(Roles = "Administrator, Project Manager")]
        // GET: Projects
        public ActionResult Index()
        {
            var projects = db.Projects.ToList();
            var viewModels = new List<ProjectManagementViewModel>();
            foreach(var project in projects){
                viewModels.Add(new ProjectManagementViewModel()
                {
                    Id=project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    Tickets = project.Tickets.ToList(),
                    UsersInProject = Helpers.ProjectHelpers.GetUsersOnProject(project.Id).ToList(),
                    UsersNotInProject = Helpers.ProjectHelpers.GetUsersNotOnProject(project.Id).ToList()
                });
            }
            return View(viewModels);
        }

        [Authorize]
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(new ProjectDetailsViewModel() {
                Project = project,
                UsersInProject = Helpers.ProjectHelpers.GetUsersOnProject(project.Id),
                UsersNotInProject = Helpers.ProjectHelpers.GetUsersNotOnProject(project.Id)
            });
        }

        // GET: Projects/Create
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,CreatedDate,UpdatedDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.CreatedDate = DateTime.Now;
                project.Users.Add(db.Users.Find(User.Identity.GetUserId()));
                var newProject = db.Projects.Add(project);
                db.SaveChanges();
                Helpers.NotificationHelpers.SendProjectNotification(newProject.Id, "Project Created!", "A project named '"+newProject.Name+"' was created!", db.Users.Find(User.Identity.GetUserId()));
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,CreatedDate,UpdatedDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.UpdatedDate = DateTime.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                Helpers.NotificationHelpers.SendProjectNotification(project.Id, "Project Edited!", "'" + project.Name + "' was edited!", db.Users.Find(User.Identity.GetUserId()));
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator, Project Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            foreach (var notification in db.Notifications.Where(x => x.ProjectId == project.Id).ToList())
            {
                db.Notifications.Remove(notification);
            }
            foreach (var ticket in db.Tickets.Where(x => x.ProjectId == project.Id))
            {
                foreach (var attachment in ticket.TicketAttachments.ToList())
                {
                    db.TicketAttachments.Remove(attachment);
                }

                foreach (var comment in ticket.TicketComments.ToList())
                {
                    db.TicketComment.Remove(comment);
                }

                foreach (var history in ticket.TicketHistorys.ToList())
                {
                    db.TicketHistory.Remove(history);
                }
                foreach(var notification in db.Notifications.Where(x => x.TicketId == ticket.Id).ToList())
                {
                    db.Notifications.Remove(notification);
                }
                db.Tickets.Remove(ticket);
            }

            foreach (var notification in db.Notifications.Where(x => x.ProjectId == project.Id).ToList())
            {
                db.Notifications.Remove(notification);
            }
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
