using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class Project
{
    public int Id { get; set; }

    public int EducationalFacilitiesId { get; set; }

    public string? TitleAr { get; set; }

    public string? TitleEn { get; set; }

    public DateOnly? Date { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public bool IsActive { get; set; }

    public virtual EducationalFacility EducationalFacilities { get; set; } = null!;

    public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();
}
