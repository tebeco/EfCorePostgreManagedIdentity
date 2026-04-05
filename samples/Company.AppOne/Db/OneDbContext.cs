using Company.AppOne.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.AppOne.Db;

public class OneDbContext(DbContextOptions<OneDbContext> options) : DbContext(options)
{
    public required DbSet<Foo> Foos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
