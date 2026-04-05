using Company.AppTwo.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.AppTwo.Db;

public class TwoDbContext : DbContext
{
    public required DbSet<Bar> Bars { get; set; }
}