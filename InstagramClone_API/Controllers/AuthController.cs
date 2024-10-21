using InstagramClone.Data;
using InstagramClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using InstagramClone_API.DTOs;
namespace InstagramClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email && u.Password == loginData.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            
            var token = GenerateJwtToken(user);

            
            var refreshToken = GenerateRefreshToken();

            
            var refreshTokenEntry = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7) 
            };
            _context.RefreshTokens.Add(refreshTokenEntry);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token,
                refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var storedToken = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
            {
                return Unauthorized(new { message = "Невалідний або неактивний рефреш-токен." });
            }

            
            var user = await _context.Users.FindAsync(storedToken.UserId);
            var newJwtToken = GenerateJwtToken(user);

            var newRefreshToken = GenerateRefreshToken();
            storedToken.Token = newRefreshToken;
            storedToken.Expires = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            var storedToken = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
            {
                return NotFound(new { message = "Токен не знайдено або неактивний." });
            }

            storedToken.Revoked = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Токен успішно відкликано." });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
