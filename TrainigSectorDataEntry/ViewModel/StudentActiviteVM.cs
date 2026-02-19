using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class StudentActiviteVM
    {

        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]

        public int EducationalFacilitiesId { get; set; }
        [ValidateNever]
        public string ImagePath { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال تفاصيل القاعات والمسارح باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? DescriptionAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال تفاصيل القاعات والمسارح باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? DescriptionEn { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public IFormFile? UploadedImage { get; set; }
        [ValidateNever]
        public virtual EducationalFacility EducationalFacilities { get; set; } = null!;
        public virtual ICollection<EntityImage> StudentActiviteImages { get; set; } = new List<EntityImage>();
        public List<IFormFile>? UploadedImages { get; set; }
        public List<int?> DeletedImageIds { get; set; } = new List<int?>();
    }
}
