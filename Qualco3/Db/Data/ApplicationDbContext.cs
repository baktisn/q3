using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Db.Models;

namespace Db.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Db.Models.Bills> Bills { get; set; }

        public DbSet<Db.Models.ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Db.Models.Settlements> Settlements { get; set; }

        public DbSet<Db.Models.SettlementTypes> SettlementTypes { get; set; }

        public DbSet<Db.Models.PaymentMethods> PaymentMethods { get; set; }

        public DbSet<Db.Models.CitizenDepts> CitizenDepts { get; set; }

        public DbSet<Db.Models.Message> Message { get; set; }
        

    }
}
