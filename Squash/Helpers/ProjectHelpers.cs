using Squash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squash.Helpers

{
    public static class ProjectHelpers
    {
        public enum IsUserOnProjectResult { ONPROJECT, OFFPROJECT, INVALID_USER, INVALID_PROJECT }
        public enum AddUserToProjectResult { INVALID_USER, INVALID_PROJECT, ALREADY_ON_PROJECT, SUCCESS}
        public enum RemoveUserFromProjectResult { INVALID_USER, INVALID_PROJECT, NOT_ON_PROJECT, SUCCESS }

        private static ApplicationDbContext db = new ApplicationDbContext();

        public static IsUserOnProjectResult IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            if (project == null) return IsUserOnProjectResult.INVALID_PROJECT;
            if (db.Users.FirstOrDefault(x => x.Id == userId) == null) return IsUserOnProjectResult.INVALID_USER;
            if (project.Users.FirstOrDefault(x => x.Id == userId) == null) return IsUserOnProjectResult.OFFPROJECT;
            return IsUserOnProjectResult.ONPROJECT;
        }

        public static ICollection<Project> ListUserProjects(string userId)
        {
            try
            {
                return db.Users.FirstOrDefault(x => x.Id == userId).Projects.ToList();
            }
            catch (System.IndexOutOfRangeException ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("User ID Given Is Invalid", "userId", ex);
                throw argEx;
            }
        }

        public static ICollection<Project> ListUserProjectsNotOn(string userId)
        {
            try
            {
                return db.Projects.Where(x => !x.Users.Any(z => z.Id == userId)).ToList();
            }
            catch (System.IndexOutOfRangeException ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("User ID Given Is Invalid", "userId", ex);
                throw argEx;
            }
        }

        public static AddUserToProjectResult AddUserToProject(string userId, int projectId)
        {
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            if (project == null) return AddUserToProjectResult.INVALID_PROJECT;
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return AddUserToProjectResult.INVALID_USER;
            if (project.Users.FirstOrDefault(x => x.Id == userId) != null) return AddUserToProjectResult.ALREADY_ON_PROJECT;
            project.Users.Add(user);
            db.SaveChanges();
            return AddUserToProjectResult.SUCCESS;
        }

        public static RemoveUserFromProjectResult RemoveUserFromProject(string userId, int projectId)
        {
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            if (project == null) return RemoveUserFromProjectResult.INVALID_PROJECT;
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return RemoveUserFromProjectResult.INVALID_USER;
            if (project.Users.FirstOrDefault(x => x.Id == userId) == null) return RemoveUserFromProjectResult.NOT_ON_PROJECT;
            project.Users.Remove(user);
            db.Entry(project).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RemoveUserFromProjectResult.SUCCESS;
        }

        public static ICollection<ApplicationUser> GetUsersOnProject(int projectId)
        {
            try
            {
                return db.Projects.FirstOrDefault(x => x.Id == projectId).Users;
            }
            catch (System.IndexOutOfRangeException ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Project ID Given Is Invalid", "projectId", ex);
                throw argEx;
            }
        }

        public static ICollection<ApplicationUser> GetUsersNotOnProject(int projectId)
        {
            try
            {
                return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
            }
            catch (System.IndexOutOfRangeException ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Project ID Given Is Invalid", "projectId", ex);
                throw argEx;
            }
        }
    }
}