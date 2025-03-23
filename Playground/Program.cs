using System.Text.Json.Serialization;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Playground.Todos;

namespace Playground;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerUI();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddDbContext<DbContext>();
        using (var context = new DbContext())
        {
            context.Database.EnsureCreated();
        }

        ProblemDetailsExtensions.AddProblemDetails(builder.Services, options =>
        {
            options.IncludeExceptionDetails = (_, _) => false;
            options.ShouldLogUnhandledException = (_, _, pd) => pd.Status == StatusCodes.Status500InternalServerError;

            options.Map<NotFoundException>(ex => new ProblemDetails
            {
                Title = "Could not find the entity",
                Status = StatusCodes.Status404NotFound,
                Detail = ex.Message,
            });

            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });

        builder.Services.AddTransient<TodoService>();

        var app = builder.Build();

        app.UseProblemDetails();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        TodoEndpoints.Map(app);

        app.Run();
    }
}
