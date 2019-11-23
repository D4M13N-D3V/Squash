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
            if (!System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Launch();
            var rolestore = new RoleStore<IdentityRole>(context);
            var rolemanager = new RoleManager<IdentityRole>(rolestore);
            var userstore = new UserStore<ApplicationUser>(context);
            var usermanager = new UserManager<ApplicationUser>(userstore);

            ApplicationUser demopm = context.Users.FirstOrDefault(x => x.Email == "projectmanager@mailinator.com");
            ApplicationUser demoadmin = context.Users.FirstOrDefault(x => x.Email == "administrator@mailinator.com");
            ApplicationUser demosubmitter = context.Users.FirstOrDefault(x => x.Email == "submitter@mailinator.com");
            ApplicationUser demodeveloper = context.Users.FirstOrDefault(x => x.Email == "developer@mailinator.com");
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
                demoadmin = user;
            }
            if (!context.Users.Any(u => u.UserName == "projectmanager@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "projectmanager@mailinator.com", Role = "Project Manager", FirstName = "Demo", LastName = "Manager", Email = "projectmanager@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Project Manager");
                demopm = user;
            }
            if (!context.Users.Any(u => u.UserName == "developer@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "developer@mailinator.com", Role = "Developer", FirstName = "Demo", LastName = "Developer", Email = "developer@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Developer");
                demodeveloper = user;
            }
            if (!context.Users.Any(u => u.UserName == "Submitter@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "Submitter@mailinator.com", Role = "Submitter", FirstName = "Demo", LastName = "Submitter", Email = "Submitter@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Submitter");
                demosubmitter = user;
            }
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
            var statuses = context.Statuses.ToList();
            var priorities = context.Priorities.ToList();
            var types = context.Types.ToList();
            var users = context.Users.ToList();
            var roles = context.Roles.ToList();




            for (int i = 0; i < 4; i++)
            {
                context.Projects.AddOrUpdate(p => p.Name, new Project() { Name = "Demo Project #" + i, Description = "The is a seeded demo project.", CreatedDate = DateTime.Now });

            }
            context.SaveChanges();
            var projects = context.Projects.ToList();
            foreach (var project in projects)
            {
                var count = 1;
                foreach(var status in statuses)
                {
                    foreach (var type in types) 
                    {
                        foreach (var priority in priorities)
                        {

                            context.Tickets.AddOrUpdate(
                                t => t.Title,
                                new Ticket
                                {
                                    ProjectId = project.Id,
                                    TypeId = type.Id,
                                    PriorityId = priority.Id,
                                    StatusId = status.Id,
                                    OwnerId = demosubmitter.Id,
                                    AssignedUserId = demodeveloper.Id,
                                    Title = project.Name+" Demo Ticket " + count,
                                    Summary = "A demo ticket of priority '" + priority.Name + "', type '" + type.Name + "', status '" + status.Name + "'",
                                    CreatedDate = DateTime.Now
                                });
                            count++;
                            context.SaveChanges();
                        }
                    }
                }
            }
            var tickets = context.Tickets.ToList();
            foreach (var ticket in tickets)
            {
                for(var i = 0; i < 4; i++)
                {
                    context.TicketComment.AddOrUpdate(
                        t => t.Id,
                        new TicketComment()
                        {
                            OwnerId = demosubmitter.Id,
                            Body = "Test Comment " + i,
                            TicketId = ticket.Id,
                            CreatedDate = DateTime.Now
                        });
                    context.TicketAttachments.AddOrUpdate(
                        t => t.Id,
                        new TicketAttachment()
                        {
                            UserId = demosubmitter.Id,
                            Description = "Test Attachment " + i,
                            TicketId = ticket.Id,
                            FilePath = "/Uploads/637081515767025812-0.jpg",
                            UploadDate = DateTime.Now
                        });
                    context.TicketHistory.AddOrUpdate(
                        t => t.Id,
                        new TicketHistory()
                        {
                            UpdaterId = demoadmin.Id,
                            Property = "Test Property",
                            OldValue = "Test Property Old",
                            NewValue = "Test Property New",
                            TicketId = ticket.Id,
                            UpdateDate = DateTime.Now
                        });
                }
            }
            context.SaveChanges();


            for (var i = 0; i < 10; i++)
            {
                context.Notifications.AddOrUpdate(
                    t => t.Title,
                    new Notification()
                    {
                        TicketId = 1,
                        IsRead = false,
                        Title = "Test Ticket Admin" + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demoadmin.Id
                    },
                    new Notification()
                    {
                        ProjectId = 1,
                        IsRead = false,
                        Title = "Test Project Admin" + (1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demoadmin.Id
                    },
                    new Notification()
                    {
                        TicketId = 1,
                        IsRead = false,
                        Title = "Test Ticket Dev" + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demodeveloper.Id
                    },
                    new Notification()
                    {
                        ProjectId = 1,
                        IsRead = false,
                        Title = "Test Project Dev" + (1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demodeveloper.Id
                    },
                    new Notification()
                    {
                        TicketId = 1,
                        IsRead = false,
                        Title = "Test Ticket PM" + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demopm.Id
                    },
                    new Notification()
                    {
                        ProjectId = 1,
                        IsRead = false,
                        Title = "Test Project PM" + (1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demopm.Id
                    },
                    new Notification()
                    {
                        TicketId = 1,
                        IsRead = false,
                        Title = "Test Ticket Sub" + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demosubmitter.Id
                    },
                    new Notification()
                    {
                        ProjectId = 1,
                        IsRead = false,
                        Title = "Test Project Sub" + (1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demosubmitter.Id
                    }
                );
                context.SaveChanges();
            }
            foreach(var project in projects)
            {
                Helpers.ProjectHelpers.AddUserToProject(demodeveloper.Id, project.Id);
                Helpers.ProjectHelpers.AddUserToProject(demosubmitter.Id, project.Id);
                Helpers.ProjectHelpers.AddUserToProject(demopm.Id, project.Id);
            }
            foreach(var ticket in tickets)
            {
                ticket.AssignedUserId = demodeveloper.Id;
                ticket.OwnerId = demosubmitter.Id;
                context.Entry(ticket).State = EntityState.Modified;
                context.SaveChanges();
            }

        }
    }
}
