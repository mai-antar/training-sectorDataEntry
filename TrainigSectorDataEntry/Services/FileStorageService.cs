using System.Text;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using TrainigSectorDataEntry.Helper;
using TrainigSectorDataEntry.Interface;
using Microsoft.Extensions.Options;


namespace TrainigSectorDataEntry.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IConfiguration _config;

        public FileStorageService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string subFolder)
        {
            string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            return await UploadFileAsync(file, subFolder, allowedImageExtensions);
        }
    
        public async Task<string?> UploadFileAsync(IFormFile file,string subFolder,string[] allowedExtensions)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return null;

            var storage = _config.GetSection("FileStorage");
            string username = storage["Username"];
            string password = storage["Password"];
            string networkPath = storage["networkPath"];

            
            string folderPath = Path.Combine(networkPath, subFolder);

            using (new NetworkShareAccesser(networkPath, username, password))
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                string fullPath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                return Path.Combine(subFolder, fileName).Replace("\\", "/");
            }
        }

        public async Task<(byte[] FileBytes, string ContentType, string FileName)?> GetFileAsync(string fileName)
        {

            fileName = Uri.UnescapeDataString(fileName);

            var storage = _config.GetSection("FileStorage");
            string username = storage["Username"];
            string password = storage["Password"];
            string networkPath = storage["networkPath"];

            string fullPath = Path.Combine(networkPath, fileName);

            using (new NetworkShareAccesser(networkPath, username, password))
            {
                if (!File.Exists(fullPath))
                    return null;

                var ext = Path.GetExtension(fullPath).ToLower();
                var contentType = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".txt" => "text/plain",
                    _ => "application/octet-stream"
                };

                var bytes = await File.ReadAllBytesAsync(fullPath);
                return (bytes, contentType, Path.GetFileName(fullPath));
            }
        }


        public async Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            var storage = _config.GetSection("FileStorage");
            string username = storage["Username"];
            string password = storage["Password"];
            string networkPath = storage["networkPath"];

            string fullPath = Path.Combine(networkPath, relativePath);

            using (new NetworkShareAccesser(networkPath, username, password))
            {
                if (File.Exists(fullPath))
                {
                    try
                    {
                        File.Delete(fullPath);
                    }
                    catch
                    {
                        // optional: log error here, don't throw
                    }
                }
            }

            await Task.CompletedTask;
        }
    }

}
