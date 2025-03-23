using Microsoft.EntityFrameworkCore;
using Playground.ToDos;

namespace Playground;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<ToDo> ToDos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=Database.db");
}
