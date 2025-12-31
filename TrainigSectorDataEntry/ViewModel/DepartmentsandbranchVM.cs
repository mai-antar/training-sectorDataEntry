using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class DepartmentsandbranchVM
    {
        public int Id { get; set; }

        public int EducationalFacilitiesId { get; set; }

        public int? DepatmentTypeID { get; set; }

        public string NameAr { get; set; } = null!;

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
