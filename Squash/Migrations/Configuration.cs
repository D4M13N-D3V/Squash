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
                var user = new ApplicationUser { UserName = "administrator@mailinator.com", FirstName = "Demo", LastName = "Admin", Email = "administrator@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Administrator");
            }
            if (!context.Users.Any(u => u.UserName == "projectmanager@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "projectmanager@mailinator.com", FirstName = "Demo", LastName = "Manager", Email = "projectmanager@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Project Manager");
            }
            if (!context.Users.Any(u => u.UserName == "developer@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "developer@mailinator.com", FirstName = "Demo", LastName = "Developer", Email = "developer@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Developer");
            }
            if (!context.Users.Any(u => u.UserName == "Submitter@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "Submitter@mailinator.com", FirstName = "Demo", LastName = "Submitter", Email = "Submitter@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Submitter");
            }
            #endregion

        }
    }
}
