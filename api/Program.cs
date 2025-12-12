using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.BusinessLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Env.Load(); // Loads the .env file into Environment variables

var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTER SERVICES ---

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// REGISTER DATABASE CONTEXT HERE
builder.Services.AddDbContext<WheresMyScheduleDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONN_STRING");
    options.UseSqlServer(connectionString);
});

// Register BLL Mode Manager as a singleton
builder.Services.AddSingleton<BllModeManager>();

// Register concrete BLL implementations as scoped services
builder.Services.AddScoped<CourseLinqBll>();
builder.Services.AddScoped<CourseSpBll>();
builder.Services.AddScoped<StudentLinqBll>();
builder.Services.AddScoped<StudentSpBll>();

// Register BLL Factory as a scoped service
builder.Services.AddScoped<BllFactory>();

// Register ICourseBll dynamically based on current BLL mode
builder.Services.AddScoped<ICourseBll>(serviceProvider =>
{
    var bllModeManager = serviceProvider.GetRequiredService<BllModeManager>();
    var bllFactory = serviceProvider.GetRequiredService<BllFactory>();
    return bllFactory.CreateCourseBll(bllModeManager.CurrentBllMode);
});

// Register IStudentBll dynamically based on current BLL mode
builder.Services.AddScoped<IStudentBll>(serviceProvider =>
{
    var bllModeManager = serviceProvider.GetRequiredService<BllModeManager>();
    var bllFactory = serviceProvider.GetRequiredService<BllFactory>();
    return bllFactory.CreateStudentBll(bllModeManager.CurrentBllMode);
});

// Register AuthService
builder.Services.AddScoped<AuthService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

var app = builder.Build();

// --- 2. MIDDLEWARE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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