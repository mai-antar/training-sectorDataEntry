using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class HistoryBreifVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم المنشأه التعليمية")]
        public int EducationalFacilitiesId { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string NameAr { get; set; } = null!;


        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة الانجليزي")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string NameEn { get; set; } = null!;


        [Required(ErrorMessage = ".برجاء ادخال عنوان النبذة التاريخية باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? TitleAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال عنوان النبذة التاريخية باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? TitleEn { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال تفاصيل النبذة التاريخية باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? DescriptionAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال تفاصيل النبذة التاريخية باللغة الانجليزية")]
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

        public virtual EducationalFacility? EducationalFacilities { get; set; } = null!;

        public virtual ICollection<HistoryBerifImage> HistoryBerifImages { get; set; } = new List<HistoryBerifImage>();

        public List<IFormFile>? UploadedImages { get; set; }
        public List<int?> DeletedImageIds { get; set; } = new List<int?>();



    }
}
