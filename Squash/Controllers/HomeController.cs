using Microsoft.AspNet.Identity;
using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Squash.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            return View(new DashboardViewModel()
            {
                FullName = user.FirstName+" "+user.LastName,
                AssignedProjects = user.Projects.ToList()
            });
        }
    }
}