using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Playground.ToDos;

public static class ToDoEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("todos", GetAsync)
            .WithName("ToDos");

        app.MapPost("todos", CreateAsync)
            .WithParameterValidation()
            .Produces(StatusCodes.Status201Created);

        app.MapDelete("todos/{id}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<List<ToDo>> GetAsync([FromServices] DbContext dbContext)
    {
        var toDos = await dbContext.ToDos
            .AsNoTracking()
            .ToListAsync();

        return toDos;
    }

    public static async Task<IResult> CreateAsync(ToDoCreateRequest request, [FromServices] DbContext dbContext)
    {
        dbContext.ToDos.Add(new ToDo
        {
            Description = request.Description!,
            DueDate = request.DueDate,
            Priority = request.Priority!.Value,
            Tags = request.Tags!,
        });

        await dbContext.SaveChangesAsync();

        return Results.Created();
    }

    private static async Task<IResult> DeleteAsync(int id, [FromServices] DbContext dbContext)
    {
        var count = await dbContext.ToDos
            .Where(i => i.Id == id)
            .ExecuteDeleteAsync();

        if (count == 0)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
