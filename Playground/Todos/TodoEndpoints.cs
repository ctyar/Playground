using Microsoft.AspNetCore.Mvc;

namespace Playground.Todos;

public static class TodoEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("todos", GetAllAsync);

        app.MapPost("todos", CreateAsync)
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        app.MapGet("todos/{id}", GetAsync)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("todos/{id}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut("todos/{id}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<List<Todo>> GetAllAsync([FromServices] ITodoService todoService,
        CancellationToken cancellationToken)
    {
        var todos = await todoService.GetAllAsync(cancellationToken);

        return todos;
    }

    private static async Task<IResult> CreateAsync(TodoRequest request, [FromServices] ITodoService todoService,
        CancellationToken cancellationToken)
    {
        await todoService.CreateAsync(request, cancellationToken);

        return Results.Created();
    }

    private static async Task<Todo> GetAsync(int id, [FromServices] ITodoService todoService,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.GetAsync(id, cancellationToken);

        return todo;
    }

    private static async Task<IResult> DeleteAsync(int id, [FromServices] ITodoService todoService,
        CancellationToken cancellationToken)
    {
        await todoService.DeleteAsync(id, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateAsync(int id, TodoRequest request, [FromServices] ITodoService todoService,
        CancellationToken cancellationToken)
    {
        await todoService.UpdateAsync(id, request, cancellationToken);

        return Results.NoContent();
    }
}
