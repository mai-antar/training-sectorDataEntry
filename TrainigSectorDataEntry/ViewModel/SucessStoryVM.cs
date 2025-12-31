using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class SucessStoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]
        public int TrainigSectorId { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال عنوان الخبر باللغة العربية")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string TitleAr { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال عنوان الخبر باللغة الانجليزية")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string TitleEn { get; set; } = null!;

        [ValidateNever]
        public string VedioPath { get; set; } = null!;
        public string? ImagePath { get; set; } = null!;
        public bool IsExternalLink { get; set; }

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
        public IFormFile? VedioFile { get; set; }
        public IFormFile? UploadedImage { get; set; }

    }
}
