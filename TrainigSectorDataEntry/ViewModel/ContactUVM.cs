using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class ContactUVM
    {
        public int Id { get; set; }


        [Required(ErrorMessage = ".برجاء اختيار اسم الجهة")]

        public int EducationalFacilitiesId { get; set; }


        [Required(ErrorMessage = ".برجاء اختيار  العنوان")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = ".برجاء ادخال رقم الهاتف ")]
        public int Telephone { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال الفاكس ")]
        public int Fax { get; set; }

        [Required(ErrorMessage = ".برجاء ادخال البريد الالكتروني ")]
        public string Email { get; set; } = null!;

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
    }
}
