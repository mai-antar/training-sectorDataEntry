using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class SpecializationVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]
        public int EducationalFacilitiesId { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة العربية")]
        //[RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string NameAr { get; set; } = null!;


        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة الانجليزي")]
        //[RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string NameEn { get; set; } = null!;

        public string? DepartmentName { get; set; }
        public string? EducationalFacilityName { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public int DepartmentsandbranchesId { get; set; }
        [ValidateNever]
        public virtual Departmentsandbranch Departmentsandbranches { get; set; } = null!;

        public virtual ICollection<SpecializationImage> SpecializationImages { get; set; } = new List<SpecializationImage>();
    }
}

