using DsiPortal.Core.ExternalEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Data
{
    public class ExternalDbContext : DbContext
    {
        public ExternalDbContext(DbContextOptions<ExternalDbContext> options) : base(options) {
            // 🔹 Bu satır EF'nin izleme davranışını tamamen kapatır.
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<MyGuide> MyGuide { get; set; }
        public DbSet<Departments> Department { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Tabloların external DB'deki gerçek adları
            modelBuilder.Entity<MyGuide>().ToTable("MyGuide");
            modelBuilder.Entity<Departments>().ToTable("Departments");

            // 🔹 Foreign key ilişkisinin doğru kurulduğundan emin ol
            modelBuilder.Entity<MyGuide>()
                .HasOne(g => g.Department)
                .WithMany()
                .HasForeignKey(g => g.DepartmentId)
                .HasPrincipalKey(d => d.Id) // 🔹 Anahtar ID
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        => throw new InvalidOperationException("ExternalDbContext read-only’dur.");

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("ExternalDbContext read-only’dur.");
    }
}
