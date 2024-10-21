using InstagramClone.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LikesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LikesController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpPost("{imageId}")]
    public async Task<IActionResult> LikeImage(int imageId)
    {
        var like = new Like
        {
            ImageId = imageId,
            UserId = 1 
        };

        _context.Likes.Add(like);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Лайк додано успішно." });
    }

    
    [HttpDelete("{imageId}")]
    public async Task<IActionResult> UnlikeImage(int imageId)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l => l.ImageId == imageId && l.UserId == 1); 

        if (like == null)
        {
            return NotFound(new { message = "Лайк не знайдено." });
        }

        _context.Likes.Remove(like);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Лайк видалено успішно." });
    }
}
