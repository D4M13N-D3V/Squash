﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime UploadDate { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}