var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// app.Urls.Add("http://localhost:3000");
// app.Urls.Add("http://localhost:4000");

// app.Run("http://localhost:5002");
app.Run();

