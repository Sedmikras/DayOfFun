using DayOfFun.Data;
using DayOfFun.Data.Services;
using DayOfFun.Data.Services.Contract;
using DayOfFun.managers;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("AuthContextConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
;
/*builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ApplicationManager>();
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    //options.CheckConsentNeeded = context => true;
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

//app.UseIdentity();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        var path = httpContext.Request.Path;
        if (!path.HasValue) return _next(httpContext);
        if (path.Value.StartsWith("/Public") || path.Value.StartsWith("/Account"))
        {
            return _next(httpContext);
        }
        else
        {
            //TODO - BLEJU
            try
            {
                if (httpContext.Session == null)
                {
                    httpContext.Response.Redirect("/Account/Login");
                }

                if (httpContext.Session.GetString("UserId") == null)
                {
                    httpContext.Response.Redirect("/Account/Login");
                }
            }
            catch (Exception e)
            {
                httpContext.Response.Redirect("/Account/Login");
            }
        }

        return _next(httpContext);
    }
}