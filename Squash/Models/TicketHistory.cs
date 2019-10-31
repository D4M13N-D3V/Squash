using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdaterId { get; set; }
        public int TicketId {get; set;}
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public virtual ApplicationUser Updater { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}