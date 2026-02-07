using BookLAB.Application;
using BookLAB.Infrastructure;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Application layer
builder.Services.AddApplicationServices();

// Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

// Seed database with initial data - DISABLED TO FIX STARTUP ISSUE
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var logger = services.GetRequiredService<ILogger<Program>>();
//     
//     try
//     {
//         logger.LogInformation("Starting database initialization...");
//         
//         var context = services.GetRequiredService<BookLABDbContext>();
//         
//         // Apply pending migrations
//         await context.Database.MigrateAsync();
//         logger.LogInformation("Database migrations applied successfully");
//         
//         // Seed data
//         await BookLABDbContextSeed.SeedAsync(context, logger);
//         logger.LogInformation("Database initialization completed");
//     }
//     catch (Exception ex)
//     {
//         logger.LogError(ex, "An error occurred while initializing the database");
//     }
// }

app.Run();
