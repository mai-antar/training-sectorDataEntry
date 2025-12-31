using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class EducationalFacility
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

    public int TrainigSectorId { get; set; }

    public virtual ICollection<AlertsAndAdvertisment> AlertsAndAdvertisments { get; set; } = new List<AlertsAndAdvertisment>();

    public virtual ICollection<Departmentsandbranch> Departmentsandbranches { get; set; } = new List<Departmentsandbranch>();

    public virtual ICollection<EducationalLevel> EducationalLevels { get; set; } = new List<EducationalLevel>();

    public virtual ICollection<HistoryBreif> HistoryBreifs { get; set; } = new List<HistoryBreif>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<QualityCertificate> QualityCertificates { get; set; } = new List<QualityCertificate>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<StudentActivite> StudentActivites { get; set; } = new List<StudentActivite>();

    public virtual ICollection<StudentsTimeTable> StudentsTimeTables { get; set; } = new List<StudentsTimeTable>();

    public virtual TrainingSector TrainigSector { get; set; } = null!;
}
