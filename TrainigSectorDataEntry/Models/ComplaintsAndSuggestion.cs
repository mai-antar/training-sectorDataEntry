using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class ComplaintsAndSuggestion
{
    public int Id { get; set; }

    public int TrainigSectorId { get; set; }

    public string Name { get; set; } = null!;

    public int Telephone { get; set; }

    public string ComplaintText { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public virtual TrainingSector TrainigSector { get; set; } = null!;
}
