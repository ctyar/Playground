using Microsoft.EntityFrameworkCore;

namespace Playground.Todos;

public class TodoService
{
    private readonly DbContext _dbContext;

    public TodoService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _dbContext.Todos
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Todo> GetAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await _dbContext.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        // To simulate an slow request
        await Task.Delay(2000, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException();
        }

        return todo;
    }

    public async Task CreateAsync(TodoRequest request, CancellationToken cancellationToken)
    {
        _dbContext.Todos.Add(new Todo
        {
            Description = request.Description!,
            DueDate = request.DueDate,
            Priority = request.Priority!.Value,
            Tags = request.Tags!,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _dbContext.Todos
            .Where(i => i.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task UpdateAsync(int id, TodoRequest request, CancellationToken cancellationToken)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException();
        }

        todo.Description = request.Description!;
        todo.DueDate = request.DueDate;
        todo.Priority = request.Priority!.Value;
        todo.Tags = request.Tags!;

        _dbContext.Update(todo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
