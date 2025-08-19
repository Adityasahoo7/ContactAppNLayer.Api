
using Microsoft.EntityFrameworkCore;

using ContactAppNLayer.DataAccess;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.DataAccess.Repositories;
using ContactAppNLayer.Services.Interfaces;
using ContactAppNLayer.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Logging Configure ----------------
//onfigure Log
// Disable default providers
builder.Logging.ClearProviders();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Default minimum level for your logs
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Fatal) // Suppress Microsoft logs
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Fatal)    // Suppress System logs
    .WriteTo.File(
        path: Path.Combine("Logs", $"log-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}",
        shared: true
    )
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("🚀 Application starting up...");



// ---------------- Service Config -------------------------

// 4. Authorization middleware
builder.Services.AddAuthorization();

// 5. Add controller services
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ContactAppNLayer.Api", Version = "v1" });

    // 🔐 JWT Authentication Support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by space and JWT token. Example: Bearer eyJhbGciOi..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



// ---------------- Dependency Injection ----------------

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"),
    b => b.MigrationsAssembly("ContactAppNLayer.DataAccess")));//This is write to store the migration in Data Access Project 

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
//This is the Dependancy Injection Performed


// ---------------- CORS ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")  // Angular origin
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

//"http://localhost:4200",

//https://contactapp.fwh.is

// ---------------- JWT Auth -----------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
            ClockSkew = TimeSpan.Zero // <-- No buffer; strict 15-min expiry
        };
    });

builder.Services.AddAuthorization();




// ---------------- Build App ----------------
var app = builder.Build();



try
{

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        Log.Information("🌐 Development environment detected at {Time}", DateTime.UtcNow);
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
    }
    else
    {
        Log.Information("🏭 Production environment detected at {Time}", DateTime.UtcNow);
    }


    // Use the CORS policy
    app.UseCors("AllowAngularApp");

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("✅ Application configured successfully at {Time}", DateTime.UtcNow);


    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly at {Time}", DateTime.UtcNow);
}
finally
{
    Log.CloseAndFlush();
}
