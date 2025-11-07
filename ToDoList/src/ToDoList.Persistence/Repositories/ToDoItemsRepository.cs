using System;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;

namespace ToDoList.Persistence.Repositories;

public class ToDoItemsRepository : IRepository<ToDoItem>
{
    private readonly ToDoItemsContext context;

    public ToDoItemsRepository(ToDoItemsContext context)
    {
        this.context = context;
    }
    public void Create(ToDoItem item)
    {
        context.ToDoItems.Add(item);
        context.SaveChanges();
    }

public IEnumerable<ToDoItem> GetAll()
    {

            var responseDtos = context.ToDoItems
                //převede každý ToDoItem z DB na ToDoItemGetResponseDto
                .Select(item => ToDoItemGetResponseDto.FromDomain(item))
                .ToList();
            return responseDtos;
        }

        public ToDoItem GetById(int id)
        {
            // Najde položku podle jejího primárního klíče
            return _context.ToDoItems.Find(id);
        }

        public void Update(ToDoItem entity)
        {
            // EF Core automaticky sleduje změny na entitě,
            // kterou jsme načetli, takže stačí jen uložit.
            // Pro jistotu můžeme explicitně nastavit stav.
            _context.ToDoItems.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            // Najdeme položku, kterou chceme smazat
            var itemToDelete = _context.ToDoItems.Find(id);
            if (itemToDelete != null)
            {
                // Pokud existuje, označíme ji ke smazání
                _context.ToDoItems.Remove(itemToDelete);
                // A provedeme smazání v databázi
                _context.SaveChanges();
            }
        }


}
