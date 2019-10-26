using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class Ticket
    {
        public Ticket()
        {
            TicketComments = new HashSet<TicketComment>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketHistorys = new HashSet<TicketHistory>();
        }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Filepath { get; set; }
        public string OwnerId { get; set; }
        public string AssignedUserId { get; set; }


        public virtual Priority Priority { get; set; }
        public virtual Status Status {get; set;}
        public virtual Type Type { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }


        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistorys { get; set; }
    }
}