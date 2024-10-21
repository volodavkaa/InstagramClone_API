using InstagramClone.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CommentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpPost("{imageId}")]
    public async Task<IActionResult> AddComment(int imageId, [FromBody] string content)
    {
        var comment = new Comment
        {
            ImageId = imageId,
            UserId = 1, 
            Content = content
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Коментар додано успішно." });
    }

    
    [HttpGet("{imageId}")]
    public async Task<IActionResult> GetComments(int imageId)
    {
        var comments = await _context.Comments
            .Where(c => c.ImageId == imageId)
            .ToListAsync();

        return Ok(comments);
    }

    
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);

        if (comment == null)
        {
            return NotFound(new { message = "Коментар не знайдено." });
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Коментар видалено успішно." });
    }
}
