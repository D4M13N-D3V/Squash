using Microsoft.AspNet.Identity;
using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Squash.Extensions
{
    public static class IdentityExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetUserFirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetUserLastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static List<Notification> GetNotifications(this IIdentity identity)
        {
            var id = identity.GetUserId();
            return db.Notifications.Where(x => x.ReciepentId == id).ToList();
        }
    }
}