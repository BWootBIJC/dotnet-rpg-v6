using System;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg
{
    public class RpgContext : DbContext
    {
        public RpgContext(DbContextOptions<RpgContext> options) : base(options)
        {
            
        }
        public DbSet<Character> Characters { get; set; }
    }
}