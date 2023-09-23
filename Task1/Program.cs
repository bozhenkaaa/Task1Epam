using Task1.Models;
using Microsoft.EntityFrameworkCore;
using Task1;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();