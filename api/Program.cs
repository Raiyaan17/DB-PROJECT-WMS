using DotNetEnv;
using Microsoft.EntityFrameworkCore; // <--- Add this namespace
using api.Models; // <--- Ensure your models are visible

Env.Load(); // Loads the .env file into Environment variables

var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTER SERVICES ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// REGISTER DATABASE CONTEXT HERE
// This tells ASP.NET: "When someone asks for WheresMyScheduleDbContext, make one using this SQL connection."
builder.Services.AddDbContext<WheresMyScheduleDbContext>(options =>
{
    // Read the connection string from the Environment Variable you set
    var connectionString = Environment.GetEnvironmentVariable("DB_CONN_STRING");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// --- 2. MIDDLEWARE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // Enable controllers

// Test Endpoint
app.MapGet("/test-db", async (WheresMyScheduleDbContext context) =>
{
    try
    {
        if (await context.Database.CanConnectAsync())
        {
            return Results.Ok("✅ Success: Database Connection Established!");
        }
        else
        {
            return Results.Problem("❌ Error: CanConnectAsync returned false.");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem($"❌ Exception: {ex.Message}");
    }
});

app.Run();