namespace TrainigSectorDataEntry.Enities
{
    public class BaseEnitiy
    {
        public int ID { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }= DateTime.Now;
        public int UpdateUserID { get; set; }
        public DateTime UpdateDate { get; set; }

        public Boolean IsActive { get; set; }=true;
        public Boolean IsDeleted { get; set; }=false;
    }
}
