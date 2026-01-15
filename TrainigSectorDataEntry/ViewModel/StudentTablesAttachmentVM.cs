using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class StudentTablesAttachmentVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار المنشأة التعليمية")]
        public int EducationalFacilitiesId { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار المرحلة التعليمية")]
        public int EducationalLevelId { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار الفصل الدراسي")]

        public int? TermsId { get; set; }


        [Required(ErrorMessage = ".برجاء اختيار نوع الجدول")]

        public int TableTypeId { get; set; }


        [Required(ErrorMessage = ".برجاء اختيار القسم")]

        public int? DepartmentsandbranchesId { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار التخصص")]

        public int? SpecializationId { get; set; }

        [ValidateNever]
        public string? FilePath { get; set; } = null!;

        public bool IsCurrent { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }
        [ValidateNever]
        public virtual EducationalLevel EducationalLevel { get; set; } = null!;
        [ValidateNever]
        public virtual EducationalFacility EducationalFacilities { get; set; } = null!;

        [ValidateNever]
        public virtual Departmentsandbranch? Departmentsandbranches { get; set; }

        [ValidateNever]
        public virtual Specialization? Specialization { get; set; }
        [ValidateNever]
        public virtual TableType TableType { get; set; } = null!;
        [ValidateNever]
        public virtual Term? Terms { get; set; }

        public IFormFile? UploadedFile { get; set; }

    

      
    }
}
