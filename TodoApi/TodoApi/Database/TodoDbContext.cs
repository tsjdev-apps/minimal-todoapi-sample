using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Database;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
    : base(options) { }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
