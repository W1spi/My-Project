using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.NLimit.Common.EntitiesModels.Sqlite;

public partial class Task
{
    [Key]
    public int TaskId { get; set; }

    [Column(TypeName = "nvarchar (40)")]
    public string TaskName { get; set; } = null!;

    [Column(TypeName = "INT")]
    public string? StudentId { get; set; }

    [Column(TypeName = "INT")]
    public int? TeacherId { get; set; }

    [Column(TypeName = "INT")]
    public int? CourseId { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Tasks")]
    public virtual Course? Course { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("Tasks")]
    [Column(TypeName = "nvarchar (40)")]
    public virtual Student? Student { get; set; }

    [ForeignKey("TeacherId")]
    [InverseProperty("Tasks")]
    public virtual Teacher? Teacher { get; set; }
}
