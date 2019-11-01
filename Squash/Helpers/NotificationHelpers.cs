using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Squash.Helpers
{
    public static class NotificationHelpers
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static void SendProjectNotification(int projectId, string title, string body, ApplicationUser sender)
        {
            var users = db.Users.Where(x => x.Projects.Any(z => projectId == z.Id)).ToList();
            //users.Remove(sender);

            List<string> emails = new List<string>();
            foreach (var user in users)
            {

                emails.Add(user.Email);
                db.Notifications.Add(new Notification()
                {
                    Title = title,
                    Body = body,
                    ProjectId = projectId,
                    ReciepentId = user.Id,
                    SentDate = DateTime.Now
                });
            }
            EmailHelpers.SendEmail(new EmailInformation()
            {
                Title = title,
                Body = body,
                Reciepents = emails
            });
            db.SaveChanges();
        }
        public static void SendTicketNotification(int ticketId, string title, string body, ApplicationUser sender)
        {
            var ticket = db.Tickets.Find(ticketId);
            var users = new List<ApplicationUser>();
            users.Add(ticket.AssignedUser);
            users.Add(ticket.Owner);
            users.Remove(sender);
            List<string> emails = new List<string>();
            foreach (var user in users)
            {
                emails.Add(user.Email);
                db.Notifications.Add(new Notification()
                {
                    Title = title,
                    Body = body,
                    TicketId = ticketId,
                    ReciepentId = user.Id,
                   SentDate = DateTime.Now
                });
            }
            EmailHelpers.SendEmail(new EmailInformation()
            {
                Title = title,
                Body = body,
                Reciepents = emails
            });
            db.SaveChanges();
        }
    }
}