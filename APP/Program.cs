using System.Linq;
using APP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BooksDB>(options =>{
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// app.MapGet("/", () => "Hello World!");
app.MapGet("/", MyApp.Hello).ExcludeFromDescription();

// Fetch all the books from the database
app.MapGet("/books", async (BooksDB db) =>
    await db.Books.ToListAsync()
).Produces<List<Book>>(StatusCodes.Status200OK)
.WithName("GetAllBooks").WithTags("Getters");

// Add new book to Sql Server DB
app.MapPost("/books",
async ([FromBody] Book addbook,[FromServices] BooksDB db, HttpResponse response) =>
{
db.Books.Add(addbook);
await db.SaveChangesAsync();
response.StatusCode = 200;
response.Headers.Location = $"books/{addbook.BookID}";
})
.Accepts<Book>("application/json")
.Produces<Book>(StatusCodes.Status201Created)
.WithName("AddNewBook").WithTags("Setters");

// pdate an existing book title using ID
app.MapPut("/books",
[AllowAnonymous] async (int bookID,string bookTitle, [FromServices] BooksDB db, HttpResponse response) =>
{
var mybook = db.Books.SingleOrDefault(s => s.BookID == bookID);
if (mybook == null) return Results.NotFound();
mybook.Title = bookTitle;
await db.SaveChangesAsync();
return Results.Created("/books",mybook);
})
.Produces<Book>(StatusCodes.Status201Created).Produces(StatusCodes.Status404NotFound)
.WithName("UpdateBook").WithTags("Setters");

// Fetch a single record using ID
app.MapGet("/books/{id}", async (BooksDB db, int id) =>
await db.Books.SingleOrDefaultAsync(s => s.BookID == id) is Book mybook ? Results.Ok(mybook) : Results.NotFound()
)
.Produces<Book>(StatusCodes.Status200OK)
.WithName("GetBookbyID").WithTags("Getters");

// Perform a search for a given keyword
app.MapGet("/books/search/{query}",
(string query, BooksDB db) =>
{
var _selectedBooks = db.Books.Where(x => x.Title.ToLower().Contains(query.ToLower())).ToList();
return _selectedBooks.Count>0? Results.Ok(_selectedBooks): Results.NotFound(Array.Empty<Book>());
})
.Produces<List<Book>>(StatusCodes.Status200OK)
.WithName("Search").WithTags("Getters");

// Get Paginated Result set
app.MapGet("/books_by_page", async (int pageNumber,int pageSize, BooksDB db) =>
await db.Books
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
.ToListAsync()
)
.Produces<List<Book>>(StatusCodes.Status200OK)
.WithName("GetBooksByPage").WithTags("Getters");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

class MyApp
{
    public static string Hello(){ 
        return "Hello World!!!"; 
    }
}