using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? ProjectId { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public DateTime SentDate { get; set; }
        public string ReciepentId { get; set; }
        public bool IsRead { get; set; }

        public virtual Project Project { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Reciepent { get; set; }
    }
}