using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Squash.Models;

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
            return RedirectToAction("Index", "Projects");
        }
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult RemoveFromProject(string userId, int projectId)
        {
            Helpers.ProjectHelpers.RemoveUserFromProject(userId, projectId);
            return RedirectToAction("Index", "Projects");
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
            return View(project);
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
                db.Projects.Add(project);
                db.SaveChanges();
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
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
