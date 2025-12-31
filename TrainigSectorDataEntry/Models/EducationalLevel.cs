using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class EducationalLevel
{
    public int Id { get; set; }

    public int EducationalFacilitiesId { get; set; }

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

    public virtual EducationalFacility EducationalFacilities { get; set; } = null!;

    public virtual ICollection<ExamSchedual> ExamScheduals { get; set; } = new List<ExamSchedual>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual ICollection<StudentsTimeTable> StudentsTimeTables { get; set; } = new List<StudentsTimeTable>();
}
