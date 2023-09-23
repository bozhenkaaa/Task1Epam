using Microsoft.EntityFrameworkCore;
using Task1.Models;

namespace Task1;

public class TodoContext: DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public DbSet<Todo> TodoItems { get; set; }
}