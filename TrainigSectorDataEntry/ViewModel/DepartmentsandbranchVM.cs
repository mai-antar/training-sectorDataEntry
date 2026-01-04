using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class DepartmentsandbranchVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم المنشأة")]
        public int EducationalFacilitiesId { get; set; }


        [Required(ErrorMessage = ".برجاء اختيار نوع القسم")]
        public int? DepatmentTypeID { get; set; }


        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة العربية")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = ".يجب كتابة لغة عربية فقط")]
        public string NameAr { get; set; } = null!;


        [Required(ErrorMessage = ".برجاء ادخال الاسم باللغة الانجليزي")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = ".يجب كتابة لغة انجليزية فقط")]
        public string NameEn { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public virtual ICollection<DepartmentsandBranchesImage> DepartmentsandBranchesImages { get; set; } = new List<DepartmentsandBranchesImage>();
        [ValidateNever]
        public virtual DepartmentType? DepatmentType { get; set; }
        [ValidateNever]
        public virtual EducationalFacility EducationalFacilities { get; set; } = null!;

        public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();


        // public IEnumerable<SelectListItem> DepartmentTypeList { get; set; } = new List<SelectListItem>();

    }
}
