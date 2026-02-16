using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class ProjectVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم المنشأة التعليمية")]
        public int EducationalFacilitiesId { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال عنوان المشروع باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? TitleAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال عنوان المشروع باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? TitleEn { get; set; }

        //[Required(ErrorMessage = "برجاء ادخال تاريخ المشروع.")]
        public DateOnly? Date { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال تفاصيل المشروع باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? DescriptionAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال تفاصيل المشروع باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? DescriptionEn { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public bool IsActive { get; set; }

        public virtual EducationalFacility? EducationalFacilities { get; set; } = null!;
        public List<EntityImage>? Images { get; set; }
        public virtual ICollection<ProjectImage> ProjectImages { get; set; } = new List<ProjectImage>();
        public List<IFormFile>? UploadedImages { get; set; }
        public List<int?> DeletedImageIds { get; set; } = new List<int?>();
    }
}
