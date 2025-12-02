namespace ToDoList.Persistence;

using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Models;

public class ToDoItemsContext : DbContext
{
    private readonly string? connectionString;
    
    public ToDoItemsContext()
    {
        // Parameterless constructor for design-time
    }
    
    public ToDoItemsContext(string connectionString = "DataSource=../../data/localdb.db")
    {
        this.connectionString = connectionString;
        this.Database.Migrate();
    }
    
    public DbSet<ToDoItem> ToDoItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (connectionString != null)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
        else
        {
            // For design-time, use default connection string
            optionsBuilder.UseSqlite("DataSource=../../data/localdb.db");
        }
    }
}
