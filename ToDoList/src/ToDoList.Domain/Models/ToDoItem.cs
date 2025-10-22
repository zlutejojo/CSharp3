namespace ToDoList.Domain.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ToDoItem
    {
        [Key]
        public int ToDoItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
