using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrdrMate.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UploadController(IWebHostEnvironment env, IConfiguration config, S3Service s3Service) : ControllerBase
{

    private readonly IWebHostEnvironment _env = env;
    private readonly S3Service _s3Service = s3Service;
    private readonly IConfiguration _config = config;

    [HttpPost("presigned-url")]
    [Authorize(Roles = "CanManageRestaurant")]
    public IActionResult GetUploadPresignedUrl([FromBody] UploadRequest request)
    {
        var fileName = $"{Guid.NewGuid()}_{request.FileName}";

        if (_env.IsDevelopment())
        {
            var uploadUrl = $"http://localhost:5126/api/upload/upload?filename={fileName}";
            var fileUrl = $"http://localhost:5126/uploads/{fileName}";

            return Ok(new
            {
                uploadUrl,
                fileUrl
            });
        }
        if (_env.IsProduction())
        {
            var bucketName = _config["AWS:BucketName"];
            if (string.IsNullOrEmpty(bucketName)) return StatusCode(500, "Bucket name is not configured.");
            var presignedUrl = _s3Service.GeneratePresignedUrl(bucketName, fileName, 15);
            return Ok(new
            {
                uploadUrl = presignedUrl,
                fileName
            });
        }

        return Forbid("Not allowed in production");
    }

    [HttpGet("presigned-url")]
    public IActionResult GetDownloadPresignedUrl(string filename)
    {
        if (_env.IsDevelopment())
        {
            var filePath = Path.Combine(_env.ContentRootPath, "uploads", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }
            var fileUrl = $"http://localhost:5126/uploads/{filename}";
            return Ok(new { fileUrl });
        }
        if (_env.IsProduction())
        {
            var bucketName = _config["AWS:BucketName"];
            if (string.IsNullOrEmpty(bucketName)) return StatusCode(500, "Bucket name is not configured.");
            var presignedUrl = _s3Service.GeneratePresignedUrl(bucketName, filename, 15);
            return Ok(new { fileUrl = presignedUrl });
        }

        return Forbid("Not allowed in production");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromQuery] string filename)
    {
        var form = await Request.ReadFormAsync();
        var file = form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        var fullPath = Path.Combine(uploadsPath, filename);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { filePath = fullPath });
    }

}

public class UploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}