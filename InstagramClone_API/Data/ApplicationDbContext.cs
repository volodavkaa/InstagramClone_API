using InstagramClone.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InstagramClone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
