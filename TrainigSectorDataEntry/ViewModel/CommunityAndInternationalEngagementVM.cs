using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class CommunityAndInternationalEngagementVM
    {
        public int Id { get; set; }





        [Required(ErrorMessage = ".برجاء ادخال العنوان باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? TitleAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال العنوان باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string? TitleEn { get; set; }

        public string? ImagePath { get; set; } = null!;


        [Required(ErrorMessage = ".برجاء ادخال التفاصيل باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string? DescriptionAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال الخبر باللغة الانجليزية")]
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
        public IFormFile? UploadedImage { get; set; }

        public bool? Type { get; set; }
    }
}
