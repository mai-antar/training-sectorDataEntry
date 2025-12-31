using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class ComplaintsAndSuggestionVM
    {
        public int Id { get; set; }

        public int TrainigSectorId { get; set; }

        public string Name { get; set; } = null!;

        public int Telephone { get; set; }

        public string ComplaintText { get; set; } = null!;

        public string Email { get; set; } = null!;
        [ValidateNever]
        public string FilePath { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }
        public IFormFile? UploadedFile { get; set; }
        [ValidateNever]

        public virtual TrainingSector TrainigSector { get; set; } = null!;
    }
}
