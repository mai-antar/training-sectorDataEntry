using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class EducationalLevelVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ".برجاء اختيار اسم المنشأة التعليمية")]
        public int EducationalFacilitiesId { get; set; }

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
        [ValidateNever]
        public virtual EducationalFacility EducationalFacilities { get; set; } = null!;
        [ValidateNever]
        public virtual ICollection<ExamSchedual> ExamScheduals { get; set; } = new List<ExamSchedual>();
        [ValidateNever]
        public virtual ICollection<Result> Results { get; set; } = new List<Result>();
        [ValidateNever]
        public virtual ICollection<StudentsTimeTable> StudentsTimeTables { get; set; } = new List<StudentsTimeTable>();
    }
}
