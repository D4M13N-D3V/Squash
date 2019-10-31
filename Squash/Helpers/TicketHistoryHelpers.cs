using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
namespace Squash.Helpers
{
    public static class TicketHistoryHelpers
    {

        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void AddHistory(string userId,int ticketId, string property, string oldValue, string newValue)
        {
            db.TicketHistory.Add(new TicketHistory()
            {
                UpdateDate = DateTime.Now,
                UpdaterId = userId,
                TicketId = ticketId,
                Property = property,
                OldValue = oldValue,
                NewValue = newValue
            });
            db.SaveChanges();
        }
    }
}