using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Db.Models;


namespace Db.Data
{
    public class DataBase  : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer("Server=tcp:qualco3.database.windows.net,1433;Initial Catalog=Qualco3;Persist Security Info=False;User ID=q3;Password=qualco123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }



        public DbSet<Db.Models.Bills> Bills { get; set; }

        public DbSet<Db.Models.ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Db.Models.Settlements> Settlements { get; set; }

        public DbSet<Db.Models.SettlementTypes> SettlementTypes { get; set; }

        public DbSet<Db.Models.PaymentMethods> PaymentMethods { get; set; }

        public DbSet<Db.Models.CitizenDepts> CitizenDepts { get; set; }

    }
}
