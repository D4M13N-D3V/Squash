using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class TicketCreationViewModel
    {
        public TicketCreationViewModel()
        {
            Ticket = new Ticket();
        }
        public Ticket Ticket { get; set; }
        public IEnumerable<Project> Projects {get; set;}
        public string ticketTitle { get; set; }
        public string ticketSummary { get; set; }
        public int projectId { get; set; }
    }
    public class TicketCreationViewModelTwo : TicketCreationViewModel
    {

    }
}