using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Priority()
        {
            Tickets = new HashSet<Ticket>();
        }
        public ICollection<Ticket> Tickets { get; set; }
    }
}