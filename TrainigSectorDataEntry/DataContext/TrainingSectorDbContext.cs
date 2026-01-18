using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.DataContext;

public partial class TrainingSectorDbContext : DbContext
{
    public TrainingSectorDbContext()
    {
    }

    public TrainingSectorDbContext(DbContextOptions<TrainingSectorDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertsAndAdvertisment> AlertsAndAdvertisments { get; set; }

    public virtual DbSet<ComplaintsAndSuggestion> ComplaintsAndSuggestions { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<DepartmentType> DepartmentTypes { get; set; }

    public virtual DbSet<DepartmentsandBranchesImage> DepartmentsandBranchesImages { get; set; }

    public virtual DbSet<Departmentsandbranch> Departmentsandbranches { get; set; }

    public virtual DbSet<EducationalFacility> EducationalFacilities { get; set; }

    public virtual DbSet<EducationalLevel> EducationalLevels { get; set; }

    public virtual DbSet<ExamSchedual> ExamScheduals { get; set; }

    public virtual DbSet<HistoryBerifImage> HistoryBerifImages { get; set; }

    public virtual DbSet<HistoryBreif> HistoryBreifs { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsImage> NewsImages { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectImage> ProjectImages { get; set; }

    public virtual DbSet<QualityCertificate> QualityCertificates { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Slider> Sliders { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<SpecializationImage> SpecializationImages { get; set; }

    public virtual DbSet<StagesAndHall> StagesAndHalls { get; set; }

    public virtual DbSet<StudentActivite> StudentActivites { get; set; }

    public virtual DbSet<StudentTablesAttachment> StudentTablesAttachments { get; set; }

    public virtual DbSet<StudentsTimeTable> StudentsTimeTables { get; set; }

    public virtual DbSet<SucessStory> SucessStories { get; set; }

    public virtual DbSet<TableType> TableTypes { get; set; }

    public virtual DbSet<Term> Terms { get; set; }

    public virtual DbSet<TrainingCourse> TrainingCourses { get; set; }

    public virtual DbSet<TrainingCoursesType> TrainingCoursesTypes { get; set; }

    public virtual DbSet<TrainingSector> TrainingSectors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        try
        {
            string c = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration =
                new ConfigurationBuilder().SetBasePath(c).AddJsonFile("appsettings.json").Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
        catch
        {
            //ignore
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertsAndAdvertisment>(entity =>
        {
            entity.ToTable("AlertsAndAdvertisment");

            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.AlertsAndAdvertisments)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlertsAndAdvertisment_EducationalFacilities");
        });

        modelBuilder.Entity<ComplaintsAndSuggestion>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.ComplaintsAndSuggestions)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplaintsAndSuggestions_TrainingSector");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.ContactUs)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContactUs_TrainingSector");
        });

        modelBuilder.Entity<DepartmentType>(entity =>
        {
            entity.ToTable("DepartmentType");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<DepartmentsandBranchesImage>(entity =>
        {
            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Departmentsandbranches).WithMany(p => p.DepartmentsandBranchesImages)
                .HasForeignKey(d => d.DepartmentsandbranchesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepartmentsandBranchesImages_Departmentsandbranches");
        });

        modelBuilder.Entity<Departmentsandbranch>(entity =>
        {
            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.DepatmentType).WithMany(p => p.Departmentsandbranches)
                .HasForeignKey(d => d.DepatmentTypeID)
                .HasConstraintName("FK_Departmentsandbranches_DepartmentType");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.Departmentsandbranches)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departmentsandbranches_EducationalFacilities");
        });

        modelBuilder.Entity<EducationalFacility>(entity =>
        {
            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.EducationalFacilities)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EducationalFacilities_TrainingSector");
        });

        modelBuilder.Entity<EducationalLevel>(entity =>
        {
            entity.ToTable("EducationalLevel");

            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.EducationalLevels)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EducationalLevel_EducationalFacilities");
        });

        modelBuilder.Entity<ExamSchedual>(entity =>
        {
            entity.ToTable("ExamSchedual");

            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.ExamScheduals)
                .HasForeignKey(d => d.EducationalLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamSchedual_EducationalLevel");
        });

        modelBuilder.Entity<HistoryBerifImage>(entity =>
        {
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.HistoryBreif).WithMany(p => p.HistoryBerifImages)
                .HasForeignKey(d => d.HistoryBreifId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistoryBerifImages_HistoryBreif");
        });

        modelBuilder.Entity<HistoryBreif>(entity =>
        {
            entity.ToTable("HistoryBreif");

            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.HistoryBreifs)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistoryBreif_EducationalFacilities");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.News)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_TrainingSector");
        });

        modelBuilder.Entity<NewsImage>(entity =>
        {
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.News).WithMany(p => p.NewsImages)
                .HasForeignKey(d => d.NewsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NewsImages_News");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.Projects)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_EducationalFacilities");
        });

        modelBuilder.Entity<ProjectImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProjectDetailsImages");

            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectImages)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectImages_Projects");
        });

        modelBuilder.Entity<QualityCertificate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Quality}ertificates");

            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.QualityCertificates)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Quality}ertificates_Quality}ertificates");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.Results)
                .HasForeignKey(d => d.EducationalLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Results_EducationalLevel");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.Services)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_EducationalFacilities");
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.ToTable("Slider");

            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.Sliders)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slider_TrainingSector");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.ToTable("Specialization");

            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Departmentsandbranches).WithMany(p => p.Specializations)
                .HasForeignKey(d => d.DepartmentsandbranchesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specialization_Departmentsandbranches");
        });

        modelBuilder.Entity<SpecializationImage>(entity =>
        {
            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Specialization).WithMany(p => p.SpecializationImages)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpecializationImages_Specialization");
        });

        modelBuilder.Entity<StagesAndHall>(entity =>
        {
            entity.ToTable(tb => tb.HasComment(""));

            entity.Property(e => e.ISStage).HasComment("");
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.StagesAndHalls)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StagesAndHalls_TrainingSector");
        });

        modelBuilder.Entity<StudentActivite>(entity =>
        {
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.StudentActivites)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentActivites_EducationalFacilities");
        });

        modelBuilder.Entity<StudentTablesAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TablesPDF");

            entity.ToTable("StudentTablesAttachment");

            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Departmentsandbranches).WithMany(p => p.StudentTablesAttachments)
                .HasForeignKey(d => d.DepartmentsandbranchesId)
                .HasConstraintName("FK_StudentTablesAttachment_Departmentsandbranches");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.StudentTablesAttachments)
                .HasForeignKey(d => d.EducationalLevelId)
                .HasConstraintName("FK_StudentTablesAttachment_EducationalLevel");

            entity.HasOne(d => d.Specialization).WithMany(p => p.StudentTablesAttachments)
                .HasForeignKey(d => d.SpecializationId)
                .HasConstraintName("FK_StudentTablesAttachment_Specialization");

            entity.HasOne(d => d.TableType).WithMany(p => p.StudentTablesAttachments)
                .HasForeignKey(d => d.TableTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentTablesAttachment_TableType");

            entity.HasOne(d => d.Terms).WithMany(p => p.StudentTablesAttachments)
                .HasForeignKey(d => d.TermsId)
                .HasConstraintName("FK_StudentTablesAttachment_Terms");
        });

        modelBuilder.Entity<StudentsTimeTable>(entity =>
        {
            entity.ToTable("StudentsTimeTable");

            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EducationalFacilities).WithMany(p => p.StudentsTimeTables)
                .HasForeignKey(d => d.EducationalFacilitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentsTimeTable_EducationalFacilities");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.StudentsTimeTables)
                .HasForeignKey(d => d.EducationalLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentsTimeTable_EducationalLevel");
        });

        modelBuilder.Entity<SucessStory>(entity =>
        {
            entity.ToTable("SucessStory");

            entity.Property(e => e.TitleAr).HasMaxLength(500);
            entity.Property(e => e.TitleEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigSector).WithMany(p => p.SucessStories)
                .HasForeignKey(d => d.TrainigSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SucessStory_TrainingSector");
        });

        modelBuilder.Entity<TableType>(entity =>
        {
            entity.ToTable("TableType");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TrainingCourse>(entity =>
        {
            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainigCoursesTypes).WithMany(p => p.TrainingCourses)
                .HasForeignKey(d => d.TrainigCoursesTypesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrainingCourses_TrainigCoursesTypes");
        });

        modelBuilder.Entity<TrainingCoursesType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TrainigCourses");

            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TrainingSector).WithMany(p => p.TrainingCoursesTypes)
                .HasForeignKey(d => d.TrainingSectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrainingCoursesTypes_TrainingSector");
        });

        modelBuilder.Entity<TrainingSector>(entity =>
        {
            entity.ToTable("TrainingSector");

            entity.Property(e => e.NameAr).HasMaxLength(500);
            entity.Property(e => e.NameEn).HasMaxLength(500);
            entity.Property(e => e.UserCreationDate).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
