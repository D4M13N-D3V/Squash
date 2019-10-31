using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class UserManagmentViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public ICollection<Project> ProjectsIn { get; set; }
        public ICollection<Project> ProjectsOut { get; set; }
        public ICollection<Ticket> TicketsIn { get; set; }
    }
}