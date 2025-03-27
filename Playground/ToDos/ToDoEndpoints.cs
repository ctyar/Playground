namespace Playground.Todos;

public static class TodoEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("todos", Create)
            .Produces(StatusCodes.Status201Created);
    }

    private static IResult Create(TodoRequest request)
    {
        return Results.Created();
    }
}
