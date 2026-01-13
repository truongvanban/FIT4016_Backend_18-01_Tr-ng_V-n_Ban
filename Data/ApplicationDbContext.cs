using FIT4016_KiemTra_2026.Models;
using Microsoft.EntityFrameworkCore;

namespace FIT4016_KiemTra_2026.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }

        // Kết nối SQL Server LocalDB (có sẵn trong Visual Studio)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SchoolManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho bảng School
            modelBuilder.Entity<School>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Principal).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Cấu hình cho bảng Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.StudentId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StudentId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Cấu hình quan hệ với School
                entity.HasOne(s => s.School)
                      .WithMany(sch => sch.Students)
                      .HasForeignKey(s => s.SchoolId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
