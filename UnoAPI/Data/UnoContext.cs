using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UnoAPI.Data.Models;

namespace UnoAPI.Data
{
    public class UnoContext : IdentityDbContext<User>
    {
        public UnoContext(DbContextOptions<UnoContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<Specie> Species { get; set; }
    }
}
