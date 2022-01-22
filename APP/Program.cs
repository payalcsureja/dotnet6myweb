using APP.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BooksDB>(options =>{
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

var app = builder.Build();

app.MapGet("/books", async (BooksDB db) =>
    await db.Books.ToListAsync()
);

// app.MapGet("/", () => "Hello World!");
app.MapGet("/", MyApp.Hello);

app.Run();

class MyApp
{
    public static string Hello(){ 
        return "Hello World!!!"; 
    }
}