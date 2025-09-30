var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/haf", () => "Haf, haf, haf!");
app.MapGet("/mnau", () => "Mňau, mňau, mňau!");
app.MapGet("/secti/{a:int}/{b:int}", (int a, int b) => $"Výsledek je {a} + {b} = {a + b}");

app.Run();
