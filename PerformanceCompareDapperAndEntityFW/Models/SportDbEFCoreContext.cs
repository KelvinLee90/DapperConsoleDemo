using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCompareDapperAndEntityFW.Models
{
    public class SportDbEFCoreContext : DbContext 
    {
        public virtual  DbSet<Sport> Sports { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        public SportDbEFCoreContext(){ }

        public SportDbEFCoreContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString.DemoDbConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Sport>().HasMany(m => m.Teams);

            builder.Entity<Team>().HasMany(m => m.Players);
        }

    }
}
