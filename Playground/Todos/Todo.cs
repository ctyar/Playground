namespace Playground.Todos;

public class Todo
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public DateTimeOffset? DueDate { get; set; }
    public Priority Priority { get; set; }
    public List<string> Tags { get; set; } = [];
}
