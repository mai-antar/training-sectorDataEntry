using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class ProjectImageVM
    {
        public int Id { get; set; }

        public int ProjectsId { get; set; }

        public string? TitleAr { get; set; }

        public string? TitleEn { get; set; }

        public string? ImagePath { get; set; } = null!;

        public string? DescriptionAr { get; set; }

        public string? DescriptionEn { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public bool IsActive { get; set; }
        [ValidateNever]
        public virtual Project Projects { get; set; } = null!;
        [Required]
        public List<IFormFile> UploadedImages { get; set; } = new();
    }
}
