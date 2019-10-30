using Microsoft.AspNet.Identity;
using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Squash.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var user = db.Users.Find(id);

            return View(new DashboardViewModel()
            {
                FullName = user.FirstName+" "+user.LastName,
                AssignedProjects = user.Projects.ToList(),
                OwnedTickets = db.Tickets.Where(x=>x.OwnerId==id).ToList()
            });
        }

        [Authorize(Roles = "Developer, Submitter")]
        public ActionResult ViewAssignedProjectsList()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var AssignedProjects = user.Projects.ToList();
            return View(AssignedProjects);
        }

        [Authorize(Roles = "Developer")]
        public ActionResult ViewAssignedTicketsList()
        {
            var id = User.Identity.GetUserId();
            var AssignedTickets = db.Tickets.Where(x => x.AssignedUserId == id).ToList();
            return View(AssignedTickets);
        }


        [Authorize]
        public ActionResult ViewAllTickets()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var Tickets = db.Tickets.ToList();
            return View(Tickets);
        }
    }
}