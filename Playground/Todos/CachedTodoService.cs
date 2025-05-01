using Microsoft.Extensions.Caching.Hybrid;

namespace Playground.Todos;

public class CachedTodoService
{
    private readonly TodoService _todoService;
    private readonly HybridCache _cache;

    public CachedTodoService(TodoService todoService, HybridCache hybridCache)
    {
        _todoService = todoService;
        _cache = hybridCache;
    }

    public async Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _cache.GetOrCreateAsync(
            "todo-all",
            async cancel => await _todoService.GetAllAsync(cancel),
            cancellationToken: cancellationToken
        );

        return result.Take(10).ToList();
    }

    public async Task<Todo> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"todo-{id}",
            async cancel => await _todoService.GetAsync(id, cancel),
            cancellationToken: cancellationToken
        );
    }
}
