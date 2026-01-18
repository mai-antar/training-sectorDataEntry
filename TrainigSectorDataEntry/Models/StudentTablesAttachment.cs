using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class StudentTablesAttachment
{
    public int Id { get; set; }

    public int? EducationalLevelId { get; set; }

    public int? TermsId { get; set; }

    public int TableTypeId { get; set; }

    public int? DepartmentsandbranchesId { get; set; }

    public int? SpecializationId { get; set; }

    public string FilePath { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? UserCreationId { get; set; }

    public DateOnly? UserCreationDate { get; set; }

    public int? UserUpdationId { get; set; }

    public DateOnly? UserUpdationDate { get; set; }

    public int? UserDeletionId { get; set; }

    public DateOnly? UserDeletionDate { get; set; }

    public virtual Departmentsandbranch? Departmentsandbranches { get; set; }

    public virtual EducationalLevel? EducationalLevel { get; set; }

    public virtual Specialization? Specialization { get; set; }

    public virtual TableType TableType { get; set; } = null!;

    public virtual Term? Terms { get; set; }
}
