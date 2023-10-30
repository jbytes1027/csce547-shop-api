using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Interfaces;
using ShopAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// use environment vars
DotNetEnv.Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();
string? connectionString = builder.Configuration["CONNECTION_STRING"];

// Get connection string from azure if production
if (builder.Environment.IsProduction())
{
    connectionString = builder.Configuration["POSTGRESQLCONNSTR_CONNECTION_STRING"];
}

if (connectionString is null) throw new Exception("No Connection String Found");

// Setup the database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

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

// Allow CORS
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
