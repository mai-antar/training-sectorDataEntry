using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class TrainingCoursesTypeVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]
        public int TrainingSectorId { get; set; }
        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة العربية")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? NameAr { get; set; }
        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة الانجليزي")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? NameEn { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public virtual ICollection<TrainingCourse> TrainingCourses { get; set; } = new List<TrainingCourse>();
        [ValidateNever]
        public virtual TrainingSector TrainingSector { get; set; } = null!;
    }
}
