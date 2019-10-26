using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public DateTime SentDate { get; set; }
        public int ReciepentId { get; set; }
        public bool IsRead { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Reciepent { get; set; }
    }
}