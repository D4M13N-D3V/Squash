﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Squash.Models
{
    public class Type
    {
        [Key]
        public int Int { get; set; }
        public string Name { get; set; }
        public Type()
        {
            Tickets = new HashSet<Ticket>();
        }
        public ICollection<Ticket> Tickets { get; set; }
    }
}