using Microsoft.Extensions.Caching.Hybrid;

namespace Playground.Todos;

public class CachedTodoService : ITodoService
{
    private readonly TodoService _todoService;
    private readonly HybridCache _cache;

    public CachedTodoService(TodoService todoService, HybridCache hybridCache)
    {
        _todoService = todoService;
        _cache = hybridCache;
    }

    public async Task CreateAsync(TodoRequest request, CancellationToken cancellationToken)
    {
        await _todoService.CreateAsync(request, cancellationToken);

        await _cache.RemoveAsync("todo-all", cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _todoService.DeleteAsync(id, cancellationToken);

        await _cache.RemoveAsync("todo-all", cancellationToken);
        await _cache.RemoveAsync($"todo-{id}", cancellationToken);
    }

    public async Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _cache.GetOrCreateAsync(
            "todo-all",
            async cancel => await _todoService.GetAllAsync(cancel),
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task<Todo> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"todo-{id}",
            async cancel => await _todoService.GetAsync(id, cancel),
            cancellationToken: cancellationToken
        );
    }

    public async Task UpdateAsync(int id, TodoRequest request, CancellationToken cancellationToken)
    {
        await _todoService.UpdateAsync(id, request, cancellationToken);

        await _cache.RemoveAsync("todo-all", cancellationToken);
        await _cache.RemoveAsync($"todo-{id}", cancellationToken);
    }
}
