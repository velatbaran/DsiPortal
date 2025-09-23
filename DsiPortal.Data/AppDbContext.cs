using DsiPortal.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Announcements> Announcements { get; set; }
        public DbSet<Apps> Apps { get; set; }
        public DbSet<FoodList> FoodList { get; set; }
        public DbSet<FoodPriceList> FoodPriceList { get; set; }
        public DbSet<Forms> Forms { get; set; }
        public DbSet<GuestFeeChart> GuestFeeChart { get; set; }
        public DbSet<Managemention> Managemention { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<WorksConducteds> WorksConducteds { get; set; }
        public DbSet<RegionalManagers> RegionalManagers { get; set; }
        public DbSet<BenefitLinks> BenefitLinks { get; set; }
        public DbSet<DepartmentManagers> DepartmentManagers { get; set; }
        public DbSet<Titles> Titles { get; set; }
        public DbSet<Cameras> Cameras { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // çalışan dll içinden configuration class ları bul
            base.OnModelCreating(modelBuilder);
        }
    }

    //public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    //{
    //    public AppDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    //        optionsBuilder.UseSqlServer("Server=B24VELATBARAN\\BT; Database=DsiPortalDb; Trusted_Connection=True; TrustServerCertificate=True;");

    //        return new AppDbContext(optionsBuilder.Options);
    //    }
    //}
}
