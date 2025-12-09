using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.HasIndex(d => d.Name).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(e => e.DocumentNumber).IsUnique();

            entity.Property(e => e.FirstName).HasMaxLength(80).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(80).IsRequired();

            entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(30);

            entity.Property(e => e.JobTitle).HasMaxLength(120);
            entity.Property(e => e.EmploymentStatus).HasMaxLength(30).IsRequired();

            entity.Property(e => e.EducationLevel).HasMaxLength(120);
            entity.Property(e => e.ProfessionalProfile).HasColumnType("TEXT");

            entity.Property(e => e.Salary).HasPrecision(18, 2);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}