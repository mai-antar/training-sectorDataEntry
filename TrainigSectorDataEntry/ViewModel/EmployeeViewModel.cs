using System.ComponentModel.DataAnnotations;

namespace TrainigSectorDataEntry.ViewModel
{
    public class EmployeeViewModel
    {
        public int ID { get; set; } // null في الإضافة، موجود في التعديل
        [Required]
        public string Name { get; set; }
        public string? PhotoPath { get; set; }
        public Boolean IsActive { get; set; } = true;
        public IFormFile? ImageFile { get; set; }  // لتحميل صورة جديدة
        
    }
}
