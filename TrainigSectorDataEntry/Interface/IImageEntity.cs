namespace TrainigSectorDataEntry.Interface
{
    public interface IImageEntity
    {
        int Id { get; set; }
        string ImagePath { get; set; }
        int ParentEntityId { get; set; }
    }
}
