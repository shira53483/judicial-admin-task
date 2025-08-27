using Microsoft.EntityFrameworkCore;
using InquiriesAPI.Models;
using InquiriesAPI.DTOs;

namespace InquiriesAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "משאבי אנוש" },
                new Department { Id = 2, Name = "טכנולוגיות מידע" },
                new Department { Id = 3, Name = "שירות לקוחות" },
                new Department { Id = 4, Name = "כספים" }
            );

            // Configure relationships
            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.Department)
                .WithMany(d => d.Inquiries)
                .HasForeignKey(i => i.DepartmentId);
        }
    }
}