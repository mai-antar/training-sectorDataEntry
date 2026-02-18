using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class NewsVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]
        public int TrainigSectorId { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال عنوان الخبر باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string TitleAr { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال عنوان الخبر باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string TitleEn { get; set; } = null!;


        //[Required(ErrorMessage = ".برجاء ادخال الخبر باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        //public string ShortDescriptionAr { get; set; } = null!;


        //[Required(ErrorMessage = ".برجاء ادخال الخبر باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        //public string ShortDescriptionEn { get; set; }
 

        [Required(ErrorMessage = ".برجاء ادخال تفاصيل الخبر باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string DescriptionAr { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال تفاصيل الخبر باللغة الانجليزية")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string DescriptionEn { get; set; }

        public DateOnly Date { get; set; }
        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<EntityImage> NewsImages { get; set; } = new List<EntityImage>();

        public virtual TrainingSector? TrainigSector { get; set; }
        public List<IFormFile>? UploadedImages { get; set; }
        public List<int?> DeletedImageIds { get; set; } = new List<int?>();

    }
}
