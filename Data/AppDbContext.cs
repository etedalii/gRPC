using LearnGrpc.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnGrpc.Data;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
}