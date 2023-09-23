using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.NLimit.Common.EntitiesModels.Sqlite;

public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    [Column(TypeName = "nvarchar (30)")]
    public string CourseName { get; set; } = null!;

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    [Column(TypeName = "image")]
    public byte[]? Picture { get; set; }

    [Column(TypeName = "INT")]
    public int? TeacherId { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    //public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    [ForeignKey("TeacherId")]
    [InverseProperty("Courses")]
    public virtual Teacher? Teacher { get; set; }
}
