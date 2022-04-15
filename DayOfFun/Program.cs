var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!");

app.Run();