using Microsoft.AspNetCore.Mvc;

namespace Playground.Todos;

public static class TodoEndpoints
{
    public static void Map(WebApplication app)
    {
        var vApi = app.NewVersionedApi();
        var group = vApi.MapGroup("api/todos");

        group.MapGet("", GetAllAsyncV1)
            .HasApiVersion(1, 0);

        group.MapGet("", GetAllAsync)
            .HasApiVersion(2, 0);

        group.MapPost("", CreateAsync)
            .HasApiVersion(1, 0)
            .HasApiVersion(2, 0)
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapGet("{id}", GetAsync)
            .HasApiVersion(1, 0)
            .HasApiVersion(2, 0)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("{id}", UpdateAsync)
            .HasApiVersion(1, 0)
            .HasApiVersion(2, 0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("{id}", DeleteAsync)
            .HasApiVersion(2, 0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<List<Todo>> GetAllAsyncV1([FromServices] CachedTodoService cachedTodoService,
        CancellationToken cancellationToken)
    {
        var todos = await cachedTodoService.GetAllAsync(cancellationToken);

        return todos;
    }

    private static async Task<List<Todo>> GetAllAsync([FromServices] CachedTodoService cachedTodoService,
        CancellationToken cancellationToken)
    {
        var todos = await cachedTodoService.GetAllAsync(cancellationToken);

        todos.Reverse();

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
