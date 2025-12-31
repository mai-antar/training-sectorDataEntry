using TrainigSectorDataEntry.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrainigSectorDataEntry.ViewModel
{
    public class TrainingCourseVM
    {
        public int Id { get; set; }

        public int TrainigCoursesTypesId { get; set; }

        public string? NameAr { get; set; }

        public string? NameEn { get; set; }
        [ValidateNever]
        public string FilePath { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        [ValidateNever]
        public virtual TrainingCoursesType TrainigCoursesTypes { get; set; } = null!;
        public IFormFile? UploadedFile { get; set; }

    }
}
