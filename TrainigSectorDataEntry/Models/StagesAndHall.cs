using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class StagesAndHall
{
    public int Id { get; set; }

    public int TrainigSectorId { get; set; }

    public string? TitleAr { get; set; }

    public string? TitleEn { get; set; }

    public string DescriptionAr { get; set; } = null!;

    public string DescriptionEn { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public bool? ISStage { get; set; }

    public virtual ICollection<StagesAndHallsImage> StagesAndHallsImages { get; set; } = new List<StagesAndHallsImage>();

    public virtual TrainingSector TrainigSector { get; set; } = null!;
}
