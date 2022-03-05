using Packt.Shared;
using static System.Console;

namespace Northwind.Web;


public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddNorthwindContext();
       
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }
        app.UseRouting();
        app.Use(async (HttpContext context, Func<Task> next) =>
        {
            RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;
            if (rep is not null)
            {
                WriteLine($"Endpoint name: {rep.DisplayName}");
                WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
            }
            if (context.Request.Path == "/bonjour")
            {
                await context.Response.WriteAsync("Bonjour Monde!");
                return;
            }
            await next();
        });
        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapGet("/hello", () => "Howdy World!");
        });
    }
}