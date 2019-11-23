using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Helpers
{
    public static class RoleHelpers
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        private static string adminId = db.Users.First(x => x.Email == "administrator@mailinator.com").Id;
        private static string pmId = db.Users.First(x => x.Email == "projectmanager@mailinator.com").Id;
        private static string devId = db.Users.First(x => x.Email == "developer@mailinator.com").Id;
        private static string subId = db.Users.First(x => x.Email == "Submitter@mailinator.com").Id;
        
            
            
            
        private static bool IsDemoUser(string id)
        {
            return (id != adminId || id != pmId || id != devId || id != subId);
        }

        public static bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }


        public static ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public static bool AddUserToRole(string userId, string roleName)
        {
            if (IsDemoUser(userId)) return false;
            var result = userManager.AddToRole(userId, roleName);
            db.Users.Find(userId).Role = roleName;
            db.SaveChanges();
            return result.Succeeded;
        }

        public static bool RemoveUserFromRole(string userId, string roleName)
        {
            if (IsDemoUser(userId)) return false;
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public static ICollection<ApplicationUser> GetUsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List) {
                if (IsUserInRole(user.Id, roleName)) resultList.Add(user);
            }
            return resultList;
        }

        public static ICollection<ApplicationUser> GetUsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName)) resultList.Add(user);
            }
            return resultList;
        }
    }
}