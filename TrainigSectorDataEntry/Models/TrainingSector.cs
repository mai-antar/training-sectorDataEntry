using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class TrainingSector
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

    public int? NumberOfSiteVisitor { get; set; }

    public virtual ICollection<ComplaintsAndSuggestion> ComplaintsAndSuggestions { get; set; } = new List<ComplaintsAndSuggestion>();

    public virtual ICollection<EducationalFacility> EducationalFacilities { get; set; } = new List<EducationalFacility>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<Slider> Sliders { get; set; } = new List<Slider>();

    public virtual ICollection<StagesAndHall> StagesAndHalls { get; set; } = new List<StagesAndHall>();

    public virtual ICollection<SucessStory> SucessStories { get; set; } = new List<SucessStory>();

    public virtual ICollection<TrainingCoursesType> TrainingCoursesTypes { get; set; } = new List<TrainingCoursesType>();
}
