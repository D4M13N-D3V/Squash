﻿using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Squash.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class UserManagmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserManagment
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            var viewModels = new List<UserManagmentViewModel>();
            foreach(var user in users)
            {
                viewModels.Add(new UserManagmentViewModel()
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    ProjectsIn = user.Projects,
                    ProjectsOut = Helpers.ProjectHelpers.ListUserProjectsNotOn(user.Id),
                    Role = Helpers.RoleHelpers.ListUserRoles(user.Id).FirstOrDefault()
                });
            }
            return View(viewModels);
        }

        [HttpPost]
        public ActionResult SetRole(string userId, string roleName)
        {
            foreach (var role in Helpers.RoleHelpers.ListUserRoles(userId))
                Helpers.RoleHelpers.RemoveUserFromRole(userId, role);
            Helpers.RoleHelpers.AddUserToRole(userId, roleName);
            return RedirectToAction("Index", "UserManagment");
        }

        [HttpPost]
        public ActionResult AddToProject(string userId, int projectId)
        {
            Helpers.ProjectHelpers.AddUserToProject(userId, projectId);
            return RedirectToAction("Index", "UserManagment");
        }
        [HttpPost]
        public ActionResult RemoveFromProject(string userId, int projectId)
        {
            Helpers.ProjectHelpers.RemoveUserFromProject(userId, projectId);
            return RedirectToAction("Index", "UserManagment");
        }

    }
}