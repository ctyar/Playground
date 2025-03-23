namespace Playground.ToDos;

public static class ToDoEndpoints
{
    public static Task<List<ToDo>> GetAsync()
    {
        return Task.FromResult(new List<ToDo>());
    }

    public static async Task CreateAsync(ToDoCreateRequest request)
    {
        throw new NotImplementedException();
    }
}
