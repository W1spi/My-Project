using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NLimit.Web.Models;

namespace NLimit.Web.Data.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.SetConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;" +
                "Initial Catalog=master;" +
                "Integrated Security=true;" +
                "Trust Server Certificate=false;");
        }

        //public static ApplicationDbContext Create() => new ApplicationDbContext();

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;" +
                "Initial Catalog=master;" +
                "Integrated Security=true;" +
                "Trust Server Certificate=false;";

            optionsBuilder.UseSqlServer(connectionString);
        }*/

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.UserId)
                .IsRequired()
                .HasColumnType("nvarchar(50)"); 

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasColumnType("nvarchar (30)");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Surname)
                .IsRequired()
                .HasColumnType("nvarchar (30)");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Patronymic)
                .HasColumnType("nvarchar (30)")
                .HasDefaultValue("NULL");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.BirthDate)
                .HasColumnType("DATETIME");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.StartDate)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Address)
                .HasColumnType("nvarchar (100)")
                .HasDefaultValue("NULL");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Email)
                .IsRequired()
                .HasColumnType("nvarchar (50)");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.MobilePhone)
                .HasColumnType("nvarchar (30)")
                .HasDefaultValue("NULL");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.AdditionalPhone)
                .HasColumnType("nvarchar (30)")
                .HasDefaultValue("NULL");

            base.OnModelCreating(modelBuilder);
        }*/
    } 
}