using DsiPortal.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
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
        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // çalışan dll içinden configuration class ları bul
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Any())
            {
                await AuditLogs.AddRangeAsync(auditEntries);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        private List<AuditLog> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLog>();
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            (e.State == EntityState.Added ||
                             e.State == EntityState.Modified ||
                             e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                var audit = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName(),
                    ChangedAt = DateTime.UtcNow,
                    UserName = "system", // burada o anki kullanıcıyı inject edebilirsin
                    Action = entry.State.ToString()
                };

                // Primary key
                var keyValues = new Dictionary<string, object>();
                foreach (var property in entry.Properties.Where(p => p.Metadata.IsPrimaryKey()))
                {
                    keyValues[property.Metadata.Name] = property.CurrentValue;
                }
                audit.KeyValues = System.Text.Json.JsonSerializer.Serialize(keyValues);

                // Added
                if (entry.State == EntityState.Added)
                {
                    var newValues = new Dictionary<string, object>();
                    foreach (var property in entry.Properties)
                    {
                        newValues[property.Metadata.Name] = property.CurrentValue;
                    }
                    audit.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);
                }
                // Deleted
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = new Dictionary<string, object>();
                    foreach (var property in entry.Properties)
                    {
                        oldValues[property.Metadata.Name] = property.OriginalValue;
                    }
                    audit.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);
                }
                // Modified
                else if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object>();
                    var newValues = new Dictionary<string, object>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            oldValues[property.Metadata.Name] = property.OriginalValue;
                            newValues[property.Metadata.Name] = property.CurrentValue;
                        }
                    }
                    audit.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);
                    audit.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);
                }

                auditEntries.Add(audit);
            }

            return auditEntries;
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
