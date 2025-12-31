using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class HistoryBerifImage
{
    public int Id { get; set; }

    public int HistoryBreifId { get; set; }

    public string ImagePath { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public virtual HistoryBreif HistoryBreif { get; set; } = null!;
}
