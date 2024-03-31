using Microsoft.EntityFrameworkCore;
using Data.NLimit.Common.EntitiesModels.SqlServer;

namespace Data.NLimit.Common.DataContext.SqlServer;
public class NLimitContext : DbContext
{
    public NLimitContext () { }
    public NLimitContext (DbContextOptions<NLimitContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Work> Works { get; set; }
    public DbSet<UserCourse> UsersCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connection = @"Data Source=(localdb)\MSSQLLocalDB;" +
            "Initial Catalog=NLimit;" +
            "Integrated Security=true;" +
            "Trust Server Certificate=true;" +
            "MultipleActiveResultSets=true;";
        
        optionsBuilder.UseSqlServer(connection);
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<User>()
            .Property(u => u.UserId)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<User>()
            .Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(30)
            .HasColumnType("nvarchar (30)");

        modelBuilder.Entity<User>()
            .Property(u => u.Surname)
            .IsRequired()
            .HasMaxLength(30)
            .HasColumnType("nvarchar (30)");

        modelBuilder.Entity<User>()
            .Property(u => u.Patronymic)
            .HasMaxLength(30)
            .HasColumnType("nvarchar (30)");

        modelBuilder.Entity<User>()
            .Property(u => u.Address)
            .HasMaxLength(100)
            .HasColumnType("nvarchar (100)");

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<User>()
            .Property(u => u.MobilePhone)
            .HasMaxLength(30)
            .HasColumnType("nvarchar (30)");

        modelBuilder.Entity<User>()
            .Property(u => u.AdditionalPhone)
            .HasMaxLength(30)
            .HasColumnType("nvarchar (30)");

        // M:M + задаем таблицу пересечений
        /*modelBuilder.Entity<User>()
            .HasMany(u => u.Course)
            .WithMany(c => c.User)
            .UsingEntity(w => w.ToTable("UsersCourses"));*/

        modelBuilder.Entity<UserCourse>()
            .Property(m => m.EnrollmentDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<UserCourse>()
            .Property(m => m.AverageScore)
            .HasMaxLength(15)
            .HasColumnType("FLOAT");

        modelBuilder.Entity<UserCourse>()
            .HasOne(m => m.User)
            .WithMany(u => u.UserCourse)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<UserCourse>()
            .HasOne(m => m.Course)
            .WithMany(c => c.UserCourse)
            .HasForeignKey(m => m.CourseId);

        modelBuilder.Entity<UserCourse>()
            .HasKey(k => new
            {
                k.UserId,
                k.CourseId
            });

        // Course
        modelBuilder.Entity<Course>()
            .HasKey(c => c.CourseId);

        modelBuilder.Entity<Course>()
            .Property(c => c.CourseId)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<Course>()
            .Property(c => c.CourseName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar (100)");

        modelBuilder.Entity<Course>()
            .Property(c => c.CourseDescription)
            .HasMaxLength(255)
            .HasColumnType("nvarchar (255)");

        // Work
        modelBuilder.Entity<Work>()
            .HasKey(w => w.WorkId);

        modelBuilder.Entity<Work>()
            .Property(w => w.WorkId)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<Work>()
            .Property(w => w.WorkName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar (100)");

        modelBuilder.Entity<Work>()
            .Property(w => w.WorkDescription)
            .HasMaxLength(255)
            .HasColumnType("nvarchar (255)");

        modelBuilder.Entity<Work>()
            .Property(w => w.CreatedBy)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<Work>()
            .Property(w => w.WorkStatus)
            .IsRequired()
            .HasMaxLength(30)
            .HasColumnType("nvarchar (30)");

        modelBuilder.Entity<Work>()
            .Property(w => w.Executor)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<Work>()
            .Property(w => w.Result)
            .HasMaxLength(20)
            .HasColumnType("nvarchar (20)");

        modelBuilder.Entity<Work>()
            .Property(w => w.FeedbackTeacher)
            .HasMaxLength(255)
            .HasColumnType("nvarchar (255)");

        modelBuilder.Entity<Work>()
            .Property(w => w.UserId)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<Work>()
            .Property(w => w.CourseId)
            .HasMaxLength(50)
            .HasColumnType("nvarchar (50)");

        modelBuilder.Entity<Work>()
            .HasOne(w => w.User)
            .WithMany(u => u.Work)
            .HasForeignKey(w => w.UserId);

        modelBuilder.Entity<Work>()
            .HasOne(w => w.Course)
            .WithMany(c => c.Work)
            .HasForeignKey(w => w.CourseId);
    }
}
