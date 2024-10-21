using InstagramClone.Data;
using InstagramClone.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmail(string email);
}

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
