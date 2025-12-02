namespace ToDoList.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ToDoList.Domain.Models;
// řeší interakci s databází pomocí Entity Framework Core
public class ToDoItemsContext : DbContext
{
    private readonly string connectionString;
    //pokud zavolám kontruktor bez parametrů, použije se výchozí connection string, pokud v konstrukturu použiji jiný connection string, použije se ten
    public ToDoItemsContext(string connectionString = "DataSource=../../data/localdb.db")
    {
        this.connectionString = connectionString;
        // Metoda, která aplikuje všechny neaplikované migrace (pokud db neexistuje, vytvoří ji a aplikuje migrace, pokud db existuje, aplikuje chybějící migrace)
        this.Database.Migrate();
    }
    // reprezentuje tabulku v databázi, která ukládá objekty ToDoItem
    public DbSet<ToDoItem> ToDoItems { get; set; }
    // nastavuje, že chci používat SQLite databázi a na které konkrétní databázi
    // tuto metodu UseSqlite můžu používat, protože mám přidaný balíček SQLite.EntityFrameworkCore
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(connectionString);

        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}
