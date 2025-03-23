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

        app.MapPut("todos/{id}", UpdateAsync)
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

    public static async Task<IResult> CreateAsync(ToDoRequest request, [FromServices] DbContext dbContext)
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

    private static async Task<IResult> UpdateAsync(int id, ToDoRequest request, [FromServices] DbContext dbContext)
    {
        var toDo = await dbContext.ToDos
            .FirstOrDefaultAsync(i => i.Id == id);

        if (toDo is null)
        {
            return Results.NotFound();
        }

        toDo.Description = request.Description!;
        toDo.DueDate = request.DueDate;
        toDo.Priority = request.Priority!.Value;
        toDo.Tags = request.Tags!;

        dbContext.Update(toDo);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
