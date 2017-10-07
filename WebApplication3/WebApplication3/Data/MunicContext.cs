using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;


namespace WebApplication3.Data
{
    public class MunicContext : DbContext
    {

        public MunicContext(DbContextOptions<MunicContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Bills> Bills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Bills>().ToTable("Bills");
        }
    }
}
