using System.ComponentModel.DataAnnotations;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.ViewModel
{
    public class EntityImageVM
    {
        public int Id { get; set; }
        public int EntityType { get; set; } 
        public int EntityId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<IFormFile> UploadedImages { get; set; } = new();
    }

    // For uploading images
    //public class EntityImageUploadVM
    //{
    //    public string EntityType { get; set; } = string.Empty;
    //    public int EntityId { get; set; }
    //    public List<IFormFile> UploadedImages { get; set; } = new();
    //}
}