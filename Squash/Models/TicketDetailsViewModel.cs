using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class TicketDetailsViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Priority> Priorities { get; set; }
        public IEnumerable<Status> Statuses { get; set; }
        public IEnumerable<Type> Types { get; set; }
        public IEnumerable<TicketHistory> History { get; set; }
        public Ticket Ticket { get; set; }
        public ApplicationUser AssignedUser { get; set;}
    }
}