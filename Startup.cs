using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register the TokenStorage service
        services.AddSingleton<TokenStorage>();

        // Register the specific provider we want to use
        services.AddScoped<IActorProvider, IMDbActorProvider>();  // Optional to use: RottenTomatoesActorProvider

        // Register ScraperService
        services.AddScoped<ScraperService>();

        // Add in-memory database context
        services.AddDbContext<ActorContext>();

        // Add controllers
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TokenStorage tokenStorage)
    {
        // Initialize the token during startup
        Task.Run(async () =>
        {
            await tokenStorage.InitializeTokenAsync();
            string token = tokenStorage.GeneratedToken;
            Console.WriteLine($"Generated Token: {tokenStorage.GeneratedToken}");
        }).Wait();

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
