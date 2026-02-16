using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;


namespace TrainigSectorDataEntry.ViewModel
{
    public class CommunityAndInternationalEngagementsImageVM
    {
        public int Id { get; set; }

        public int CommunityAndInternationalEngagementsId { get; set; }
        public string Name { get; set; }

        public string? ImagePath { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UserCreationId { get; set; }

        public DateOnly? UserCreationDate { get; set; }

        public int? UserUpdationId { get; set; }

        public DateOnly? UserUpdationDate { get; set; }

        public int? UserDeletionId { get; set; }

        public DateOnly? UserDeletionDate { get; set; }

        public bool IsActive { get; set; }

        public virtual StagesAndHall? StagesAndHalls { get; set; } = null!;

        [Required]
        public List<IFormFile> UploadedImages { get; set; } = new();
    }
}
