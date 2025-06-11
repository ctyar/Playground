
namespace Playground.Todos;

public interface ITodoService
{
    Task CreateAsync(TodoRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken);
    Task<Todo> GetAsync(int id, CancellationToken cancellationToken);
    Task UpdateAsync(int id, TodoRequest request, CancellationToken cancellationToken);
}