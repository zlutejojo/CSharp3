namespace ToDoList.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ToDoItemsContextFactory : IDesignTimeDbContextFactory<ToDoItemsContext>
{
    public ToDoItemsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ToDoItemsContext>();
        optionsBuilder.UseSqlite("DataSource=../../data/localdb.db");

        return new ToDoItemsContext();
    }
}

