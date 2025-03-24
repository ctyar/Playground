using Microsoft.AspNetCore.Mvc;

namespace Playground.Todos;

public static class TodoEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("todos", GetAllAsync);

        app.MapPost("todos", CreateAsync)
            .WithParameterValidation()
            .Produces(StatusCodes.Status201Created);

        app.MapGet("todos/{id}", GetAsync)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("todos/{id}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut("todos/{id}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<List<Todo>> GetAllAsync([FromServices] CachedTodoService cachedTodoService,
        CancellationToken cancellationToken)
    {
        var todos = await cachedTodoService.GetAllAsync(cancellationToken);

        return todos;
    }

    private static async Task<IResult> CreateAsync(TodoRequest request, [FromServices] TodoService todoService,
        CancellationToken cancellationToken)
    {
        await todoService.CreateAsync(request, cancellationToken);

        return Results.Created();
    }

    private static async Task<Todo> GetAsync(int id, [FromServices] CachedTodoService cachedTodoService,
        CancellationToken cancellationToken)
    {
        var todo = await cachedTodoService.GetAsync(id, cancellationToken);

        return todo;
    }

    private static async Task<IResult> DeleteAsync(int id, [FromServices] TodoService todoService,
        CancellationToken cancellationToken)
    {
        await todoService.DeleteAsync(id, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateAsync(int id, TodoRequest request, [FromServices] TodoService todoService,
        CancellationToken cancellationToken)
    {
        await todoService.UpdateAsync(id, request, cancellationToken);

        return Results.NoContent();
    }
}
