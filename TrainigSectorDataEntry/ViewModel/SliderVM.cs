using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class SliderVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]
        public int TrainigSectorId { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال العنوان باللغة العربية")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string TitleAr { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال العنوان باللغة الانجليزية")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string TitleEn { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال التفاصيل باللغة العربية")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string DescriptionAr { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال التفاصيل باللغة الانجليزية")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string DescriptionEn { get; set; } = null!;
        [ValidateNever]
        public string FilePath { get; set; } = null!;
        public bool IsVideo { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }
        [ValidateNever]
        public virtual TrainingSector TrainigSector { get; set; } = null!;
        public IFormFile? UploadedFile { get; set; }
    }
}
