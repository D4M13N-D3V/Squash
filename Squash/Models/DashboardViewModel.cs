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
    }
}