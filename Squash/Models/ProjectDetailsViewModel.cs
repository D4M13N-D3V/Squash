using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }
        public ICollection<ApplicationUser> UsersNotInProject { get; set; }
        public ICollection<ApplicationUser> UsersInProject { get; set; }
    }
}