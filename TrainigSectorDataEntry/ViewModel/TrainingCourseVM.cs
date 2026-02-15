using TrainigSectorDataEntry.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TrainigSectorDataEntry.ViewModel
{
    public class TrainingCourseVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار نوع الدورة التدريبية")]
        public int TrainigCoursesTypesId { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? NameAr { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة الانجليزي")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? NameEn { get; set; }
        [ValidateNever]
        public string FilePathAr { get; set; } = null!;

        public string? FilePathEn { get; set; }

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
        public IFormFile? UploadedFileAr { get; set; }
        public IFormFile? UploadedFileEn { get; set; }
    

    }
}
