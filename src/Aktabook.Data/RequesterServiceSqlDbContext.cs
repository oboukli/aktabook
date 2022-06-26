using Microsoft.EntityFrameworkCore;

namespace Aktabook.Data;

public sealed class RequesterServiceSqlDbContext : RequesterServiceDbContext
{
    public RequesterServiceSqlDbContext(
        DbContextOptions<RequesterServiceSqlDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
    }
}