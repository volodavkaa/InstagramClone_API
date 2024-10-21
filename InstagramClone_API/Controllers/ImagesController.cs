using InstagramClone.Data;
using InstagramClone.Models; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ImagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string caption)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Немає файлу для завантаження." });
        }

       
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads"));
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        
        var image = new Image
        {
            UserId = 1, 
            FilePath = filePath,
            Caption = caption,
            CreatedAt = DateTime.UtcNow 
        };

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Зображення завантажене успішно.", image.FilePath });
    }


    [HttpGet]
    public async Task<IActionResult> GetImages()
    {
        var images = await _context.Images.ToListAsync();
        return Ok(images);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetImage(int id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
        {
            return NotFound(new { message = "Зображення не знайдено." });
        }

        return Ok(image);
    }
}
