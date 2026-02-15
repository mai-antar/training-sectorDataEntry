using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class SucessStory
{
    public int Id { get; set; }

    public int TrainigSectorId { get; set; }

    public string TitleAr { get; set; } = null!;

    public string TitleEn { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public string? VedioPath { get; set; }

    public bool? IsExternalLink { get; set; }

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public virtual TrainingSector TrainigSector { get; set; } = null!;
}
