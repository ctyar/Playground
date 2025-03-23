using Microsoft.AspNetCore.Mvc;

namespace Playground.Todos;

public static class TodoEndpoints
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

    public static async Task<List<Todo>> GetAsync([FromServices] TodoService todoService)
    {
        var todos = await todoService.GetAsync();

        return todos;
    }

    public static async Task<IResult> CreateAsync(TodoRequest request, [FromServices] TodoService todoService)
    {
        await todoService.CreateAsync(request);

        return Results.Created();
    }

    private static async Task<IResult> DeleteAsync(int id, [FromServices] TodoService todoService)
    {
        await todoService.DeleteAsync(id);

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateAsync(int id, TodoRequest request, [FromServices] TodoService todoService)
    {
        await todoService.UpdateAsync(id, request);

        return Results.NoContent();
    }
}
