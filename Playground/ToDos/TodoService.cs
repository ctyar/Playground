using Microsoft.EntityFrameworkCore;

namespace Playground.Todos;

public class TodoService
{
    private readonly DbContext _dbContext;

    public TodoService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Todo>> GetAsync()
    {
        var result = await _dbContext.Todos
            .AsNoTracking()
            .ToListAsync();

        return result;
    }

    public async Task CreateAsync(TodoRequest request)
    {
        _dbContext.Todos.Add(new Todo
        {
            Description = request.Description!,
            DueDate = request.DueDate,
            Priority = request.Priority!.Value,
            Tags = request.Tags!,
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _dbContext.Todos
            .Where(i => i.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task UpdateAsync(int id, TodoRequest request)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(i => i.Id == id);

        if (todo is null)
        {
            throw new NotFoundException();
        }

        todo.Description = request.Description!;
        todo.DueDate = request.DueDate;
        todo.Priority = request.Priority!.Value;
        todo.Tags = request.Tags!;

        _dbContext.Update(todo);
        await _dbContext.SaveChangesAsync();
    }
}
