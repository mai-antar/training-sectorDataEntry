using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class Specialization
{
    public int Id { get; set; }

    public string NameAr { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public int DepartmentsandbranchesId { get; set; }

    public virtual Departmentsandbranch Departmentsandbranches { get; set; } = null!;

    public virtual ICollection<SpecializationImage> SpecializationImages { get; set; } = new List<SpecializationImage>();
}
