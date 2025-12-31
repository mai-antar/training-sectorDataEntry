namespace TrainigSectorDataEntry.Interface
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string subFolder);
        Task<string?> UploadFileAsync(IFormFile file, string subFolder, string[] allowedExtensions);
        Task<(byte[] FileBytes, string ContentType, string FileName)?> GetFileAsync(string fileName);
        Task DeleteFileAsync(string relativePath);
    }
}
