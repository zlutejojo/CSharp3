namespace ToDoList.Frontend.Models
{
using System.ComponentModel.DataAnnotations;

    public class ToDoItemView
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is mandatory.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is mandatory.")]
        [StringLength(250, ErrorMessage = "Description cannot be longer than 250 characters.")]
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string? Category { get; set; }
    }
}
