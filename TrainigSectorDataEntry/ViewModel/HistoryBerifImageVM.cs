using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class HistoryBerifImageVM
    {
        public int Id { get; set; }

        public int HistoryBreifId { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public virtual HistoryBreif? HistoryBreif { get; set; } = null!;

        [Required]
        public List<IFormFile> UploadedImages { get; set; } = new();
    }
}
