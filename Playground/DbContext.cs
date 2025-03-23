using Microsoft.EntityFrameworkCore;
using Playground.Todos;

namespace Playground;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Todo> Todos { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=Database.db");
}
