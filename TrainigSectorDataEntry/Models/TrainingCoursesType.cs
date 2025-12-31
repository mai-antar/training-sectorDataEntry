using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class TrainingCoursesType
{
    public int Id { get; set; }

    public string? NameAr { get; set; }

    public string? NameEn { get; set; }

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public int TrainingSectorId { get; set; }

    public virtual ICollection<TrainingCourse> TrainingCourses { get; set; } = new List<TrainingCourse>();

    public virtual TrainingSector TrainingSector { get; set; } = null!;
}
