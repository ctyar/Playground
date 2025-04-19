using System.Collections.ObjectModel;

namespace Playground;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddValidation();

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerUI();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapSwaggerUI();
        }

        app.MapPost("todos", (Todo todo) =>
        {
            return Results.Created();
        });

        app.Run();
    }
}

public class Todo
{
    public DateTimeOffset DueDateTimeOffset { get; set; }
    public DateOnly DueDateOnly { get; set; }
    //public TimeOnly DueTimeOnly { get; set; }
}
