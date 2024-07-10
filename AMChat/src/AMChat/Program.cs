using AMChat;
using AMChat.Application;
using AMChat.Infrastructure;
using AMChat.Infrastructure.Persistence.Seeding.Initializers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((context, configuration) =>
                            configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices();

var app = builder.Build();

// Initialise and seed database
using (var scope = app.Services.CreateScope())
{
    var appDbInitializer = scope.ServiceProvider
        .GetRequiredService<IAppDbContextInitializer>();

    await appDbInitializer.ApplyDatabaseStructure();

    if (args.Contains("/seed"))
    {
        await appDbInitializer.SeedAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

namespace AMChat
{
    public partial class Program;
}
