using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.NLimit.Common.EntitiesModels.Sqlite;

public partial class NlimitContext : DbContext
{
    public NlimitContext()
    {
    }

    public NlimitContext(DbContextOptions<NlimitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    //public virtual DbSet<CoursesStudent> CoursesStudents { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = @"C:\Users\stepa\source\repos\CognitionVer3\Data\NLimit.db";
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Teachers

        modelBuilder.Entity<Teacher>()
            .Property(t => t.TeacherId)
            .IsRequired();

        modelBuilder.Entity<Teacher>()
            .Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.MiddleName)
            .HasMaxLength(20);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Address)
            .HasMaxLength(60);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.City)
            .HasMaxLength(15);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Country)
            .HasMaxLength(15);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Region)
            .HasMaxLength(15);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.HomePhone)
            .HasMaxLength(24);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.MobilePhone)
            .HasMaxLength(24);

        // 1:М (Teachers - Students)
        modelBuilder.Entity<Teacher>()
            .HasMany(t => t.Students)
            .WithOne(s => s.Teacher);

        // 1:М (Teachers - Courses)
        modelBuilder.Entity<Teacher>()
            .HasMany(t => t.Courses)
            .WithOne(c => c.Teacher);

        // 1:М (Teachers - Tasks)
        modelBuilder.Entity<Teacher>()
            .HasMany(teacher => teacher.Tasks)
            .WithOne(task => task.Teacher);


        // Students

        modelBuilder.Entity<Student>()
            .Property(s => s.StudentId)
            .IsRequired();

        modelBuilder.Entity<Student>()
            .Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Student>()
            .Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Student>()
            .Property(s => s.MiddleName)
            .HasMaxLength(20);

        modelBuilder.Entity<Student>()
            .Property(s => s.Address)
            .HasMaxLength(60);

        modelBuilder.Entity<Student>()
            .Property(s => s.City)
            .HasMaxLength(15);

        modelBuilder.Entity<Student>()
            .Property(s => s.Region)
            .HasMaxLength(15);

        modelBuilder.Entity<Student>()
            .Property(s => s.Country)
            .HasMaxLength(15);

        modelBuilder.Entity<Student>()
            .Property(s => s.HomePhone)
            .HasMaxLength(24);

        modelBuilder.Entity<Student>()
            .Property(s => s.MobilePhone)
            .HasMaxLength(24);

        // 1:М (Students - Teachers)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Teacher)
            .WithMany(t => t.Students);

        // 1:M (Students - Tasks)
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Tasks)
            .WithOne(t => t.Student);

        // Courses

        modelBuilder.Entity<Course>()
            .Property(c => c.CourseId)
            .IsRequired();

        modelBuilder.Entity<Course>()
            .Property (c => c.CourseName)
            .IsRequired()
            .HasMaxLength(30);

        // 1:M (Courses - Teachers)
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Courses);

        // 1:M (Courses - Tasks)
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Tasks)
            .WithOne(t => t.Course);

        // Tasks

        modelBuilder.Entity<Task>()
            .Property(t => t.TaskId)
            .IsRequired();

        modelBuilder.Entity<Task>()
            .Property(t => t.TaskName)
            .IsRequired()
            .HasMaxLength(40);

        // 1:M (Tasks - Teacher)
        modelBuilder.Entity<Task>()
            .HasOne(task => task.Teacher)
            .WithMany(teacher => teacher.Tasks);

        // 1:M (Tasks - Students)
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Student)
            .WithMany(s => s.Tasks);

        // 1:M (Tasks - Courses)
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Course)
            .WithMany(c => c.Tasks);

        // CoursesStudents - временно удалил таблицу

        /*modelBuilder.Entity<CoursesStudent>()
            .HasKey(cs => new
            {
                cs.StudentId,
                cs.CourseId
            }); */

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
