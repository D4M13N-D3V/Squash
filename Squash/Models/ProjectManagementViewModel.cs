using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class ProjectManagementViewModel
    {
        public ProjectManagementViewModel()
        {
            Tickets = new List<Ticket>();
            UsersInProject = new List<ApplicationUser>();
            UsersNotInProject = new List<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<ApplicationUser> UsersInProject { get; set; }
        public List<ApplicationUser> UsersNotInProject { get; set; }
    }
}