using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class DashboardViewModel
    {
        public string FullName { get; set; }
        public List<Project> AssignedProjects { get; set; }
        public List<Ticket> OwnedTickets { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Priority> Priorities { get; set; }
        public List<Type> Types { get; set; }
        public List<Project> Projects { get; set; }
    }
}