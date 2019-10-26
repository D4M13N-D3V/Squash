using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Status()
        {
            Tickets = new HashSet<Ticket>();
        }
        public ICollection<Ticket> Tickets { get; set; }
    }
}