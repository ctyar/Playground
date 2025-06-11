using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Playground.Todos;
using ZiggyCreatures.Caching.Fusion;

namespace Playground;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddFusionCache().
            WithOptions(o =>
            {
                o.DefaultEntryOptions.Duration = TimeSpan.FromMinutes(30);
            })
            .AsHybridCache();

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        QueueLimit = 1,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        PermitLimit = 10, // 10 req per 10 second
                        Window = TimeSpan.FromSeconds(10),
                    }));

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi(o =>
        {
            o.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Servers = [];
                return Task.CompletedTask;
            });
        });
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

            options.Map<ProblemDetailsException>(ex => new ProblemDetails
            {
                Title = ex.ProblemDetails.Title,
                Status = ex.ProblemDetails.Status,
                Detail = ex.ProblemDetails.Detail,
            });

            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });

        builder.Services.AddTransient<TodoService>();
        builder.Services.AddTransient<ITodoService, CachedTodoService>();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        app.UseProblemDetails();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseRateLimiter();

        TodoEndpoints.Map(app);

        app.Run();
    }
}
