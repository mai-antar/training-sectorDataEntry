using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class TrainingCourse
{
    public int Id { get; set; }

    public int TrainigCoursesTypesId { get; set; }

    public string? NameAr { get; set; }

    public string? NameEn { get; set; }

    public string? FilePathEn { get; set; }

    public string FilePathAr { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public virtual TrainingCoursesType TrainigCoursesTypes { get; set; } = null!;
}
