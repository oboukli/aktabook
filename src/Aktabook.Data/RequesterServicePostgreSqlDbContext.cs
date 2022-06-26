using Microsoft.EntityFrameworkCore;

namespace Aktabook.Data;

public sealed class
    RequesterServicePostgreSqlDbContext : RequesterServiceDbContext
{
    public RequesterServicePostgreSqlDbContext(
        DbContextOptions<RequesterServicePostgreSqlDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
    }
}