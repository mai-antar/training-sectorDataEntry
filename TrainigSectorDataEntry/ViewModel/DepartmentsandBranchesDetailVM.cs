using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class DepartmentsandBranchesDetailVM
    {
        public int Id { get; set; }

        public int DepartmentsandBranchesId { get; set; }

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
        [ValidateNever]
        public virtual Departmentsandbranch DepartmentsandBranches { get; set; } = null!;
        [ValidateNever]
        public virtual ICollection<DepartmentsandBranchesImage> DepartmentsandBranchesImages { get; set; } = new List<DepartmentsandBranchesImage>();
        public List<IFormFile>? UploadedImages { get; set; }
        public List<int?> DeletedImageIds { get; set; } = new List<int?>();
    }
}
