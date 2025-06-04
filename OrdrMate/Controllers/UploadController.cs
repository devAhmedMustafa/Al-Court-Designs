using Microsoft.AspNetCore.Mvc;

namespace OrdrMate.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UploadController : ControllerBase
{

    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public UploadController(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _config = config;
    }

    [HttpPost("presigned-url")]
    public IActionResult GetUploadInfo([FromBody] UploadRequest request)
    {
        var fileName = $"{Guid.NewGuid()}_{request.FileName}";
        var fileType = request.FileType;

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