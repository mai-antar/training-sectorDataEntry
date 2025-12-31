namespace TrainigSectorDataEntry.Models
{
    public class BaseEnitiy
    {
        public int ID { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int UpdateUserID { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
