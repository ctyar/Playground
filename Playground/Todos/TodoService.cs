using Microsoft.AspNetCore.Mvc;
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
        // To simulate a slow request
        await Task.Delay(2000, cancellationToken);

        return await _dbContext.Todos
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Todo> GetAsync(int id, CancellationToken cancellationToken)
    {
        // To simulate a slow request
        await Task.Delay(2000, cancellationToken);

        var todo = await _dbContext.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (todo is null)
        {
            throw new ProblemDetailsException(new ProblemDetails
            {
                Title = "Could not find the todo",
                Status = StatusCodes.Status404NotFound,
            });
        }

        return todo;
    }

    public async Task CreateAsync(TodoRequest request, CancellationToken cancellationToken)
    {
        // TODO: Invalidate the cache
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
            throw new ProblemDetailsException(new ProblemDetails
            {
                Title = "Could not find the todo",
                Status = StatusCodes.Status404NotFound,
            });
        }

        todo.Description = request.Description!;
        todo.DueDate = request.DueDate;
        todo.Priority = request.Priority!.Value;
        todo.Tags = request.Tags!;

        _dbContext.Update(todo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
