using Microsoft.EntityFrameworkCore;
using WebApplication1.context;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

using (var ctx = new QuizContext())
{
    ctx.Database.EnsureDeleted();
    ctx.Database.Migrate();
}

/*app.UseAuthentication();
app.UseAuthorization();*/
app.MapGet("/", () => "Hello World!");

app.Run();