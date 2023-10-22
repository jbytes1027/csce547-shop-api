using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Interfaces;
using ShopAPI.Services;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();



// Setup the database
string? connectionString = builder.Configuration["CONNECTION_STRING"];
if (connectionString is null) throw new Exception("No Connection String Found");
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
