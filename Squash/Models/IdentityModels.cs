using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Squash.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            userIdentity.AddClaim(new Claim("LastName", LastName));
            return userIdentity;
        }

        public ApplicationUser()
        {
            CreatedTickets = new HashSet<Ticket>();
            Projects = new HashSet<Project>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }    
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        public virtual ICollection<Ticket> CreatedTickets { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketComment> TicketComment { get; set; }
        public DbSet<TicketHistory> TicketHistory { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();  
        }

    }
}