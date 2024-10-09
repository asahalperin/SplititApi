using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register the specific provider we want to use
        // Example: IMDbActorProvider is injected
        services.AddScoped<IActorProvider, IMDbActorProvider>();  // Optional to use: RottenTomatoesActorProvider

        // Register ScraperService
        services.AddScoped<ScraperService>();

        // Add in-memory database context
        services.AddDbContext<ActorContext>();

        // Add controllers
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Show error while using development env
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
