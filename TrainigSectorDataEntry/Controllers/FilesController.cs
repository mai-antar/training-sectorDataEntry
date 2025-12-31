using Microsoft.AspNetCore.Mvc;
using TrainigSectorDataEntry.Interface;

[Route("files")]
public class FilesController : Controller
{
    
    private readonly IFileStorageService _fileStorageService;
    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{*filePath}")]
    public async Task<IActionResult> Download(string filePath)
    {
        var result = await _fileStorageService.GetFileAsync(filePath);

        if (result == null)
            return NotFound("File not found");

        return File(result.Value.FileBytes,
                    result.Value.ContentType,
                    result.Value.FileName);

    }
}
