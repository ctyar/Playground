using System.ComponentModel.DataAnnotations;

namespace Playground.ToDos;

public class ToDoRequest
{
    [Required]
    public string? Description { get; set; } = null!;
    public DateTimeOffset? DueDate { get; set; }
    [Required]
    public Priority? Priority { get; set; }
    public List<string>? Tags { get; set; } = [];
}
