
using Microsoft.EntityFrameworkCore;

using ContactAppNLayer.DataAccess;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.DataAccess.Repositories;
using ContactAppNLayer.Services.Interfaces;
using ContactAppNLayer.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependancy Injection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"),
    b => b.MigrationsAssembly("ContactAppNLayer.DataAccess")));//This is write to store the migration in Data Access Project 

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
//This is the Dependancy Injection Performed


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
