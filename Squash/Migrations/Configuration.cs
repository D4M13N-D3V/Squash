namespace Squash.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Squash.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Squash.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Squash.Models.ApplicationDbContext context)
        {
            var rolestore = new RoleStore<IdentityRole>(context);
            var rolemanager = new RoleManager<IdentityRole>(rolestore);
            var userstore = new UserStore<ApplicationUser>(context);
            var usermanager = new UserManager<ApplicationUser>(userstore);

            #region Role Seeding
            if (!context.Roles.Any(r => r.Name == "Administrator")) 
                rolemanager.Create(new IdentityRole { Name = "Administrator" });
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
                rolemanager.Create(new IdentityRole { Name = "Project Manager" });
            if (!context.Roles.Any(r => r.Name == "Developer"))
                rolemanager.Create(new IdentityRole { Name = "Developer" });
            if (!context.Roles.Any(r => r.Name == "Submitter"))
                rolemanager.Create(new IdentityRole { Name = "Submitter" });
            #endregion

            #region Demo User Seeds
            if (!context.Users.Any(u => u.UserName == "administrator@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "administrator@mailinator.com", Role="Administrator", FirstName = "Demo", LastName = "Admin", Email = "administrator@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Administrator");
            }
            if (!context.Users.Any(u => u.UserName == "projectmanager@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "projectmanager@mailinator.com", Role = "Project Manager", FirstName = "Demo", LastName = "Manager", Email = "projectmanager@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Project Manager");
            }
            if (!context.Users.Any(u => u.UserName == "developer@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "developer@mailinator.com", Role = "Developer", FirstName = "Demo", LastName = "Developer", Email = "developer@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Developer");
            }
            if (!context.Users.Any(u => u.UserName == "Submitter@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "Submitter@mailinator.com", Role = "Submitter", FirstName = "Demo", LastName = "Submitter", Email = "Submitter@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Submitter");
            }
            #endregion





            #region Project Seed
            context.Projects.AddOrUpdate(
                p => p.Name,
                new Project() { Name = "First Demo", Description = "The first demo project seed", CreatedDate = DateTime.Now },
                new Project() { Name = "Second Demo", Description = "The second demo project seed", CreatedDate = DateTime.Now },
                new Project() { Name = "Third Demo", Description = "The third demo project seed", CreatedDate = DateTime.Now },
                new Project() { Name = "Fourth Demo", Description = "The fourth demo project seed", CreatedDate = DateTime.Now });
            #endregion
            #region Ticket Status Seed
            context.Statuses.AddOrUpdate(
                s => s.Name,
                new Status() { Name = "Created"},
                new Status() { Name = "Assigned" },
                new Status() { Name = "Resolved" },
                new Status() { Name = "Reopened"},
                new Status() { Name = "Archived"});
            #endregion
            #region Ticket Priority Seed
            context.Priorities.AddOrUpdate(
                p => p.Name,
                new Priority() { Name = "Low"},
                new Priority() { Name = "Medium" },
                new Priority() { Name = "High"});
            #endregion
            #region Ticket Type Seed
            context.Types.AddOrUpdate(
                p => p.Name,
                new Models.Type() { Name = "Software"},
                new Models.Type() { Name = "Hardware" },
                new Models.Type() { Name = "UI" },
                new Models.Type() { Name = "Other" });
            #endregion

            context.SaveChanges();
            var projects = context.Projects;
            var statuses = context.Statuses;
            var priorities = context.Priorities;
            var types = context.Types;
            var users = context.Users;
            var roles = context.Roles;

            context.Tickets.AddOrUpdate(
                t => t.Title,
                new Ticket
                {
                    ProjectId = projects.FirstOrDefault(p => p.Name.Contains("First")).Id,
                    TypeId = types.FirstOrDefault(t => t.Name.Contains("Software")).Id,
                    PriorityId = priorities.FirstOrDefault(p => p.Name.Contains("High")).Id,
                    StatusId = statuses.FirstOrDefault(s => s.Name.Contains("Assigned")).Id,
                    OwnerId = users.FirstOrDefault(u => u.Email.Contains("submitter@mailinator.com")).Id,
                    AssignedUserId = users.FirstOrDefault(u => u.Email.Contains("developer@mailinator.com")).Id,
                    Title = "Demo Ticket 1",
                    Summary = "A demo ticket that beings assigned to a developer at high priority",
                    CreatedDate = DateTime.Now
                });
            context.SaveChanges();
        }
    }
}
