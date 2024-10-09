using SplititScraperApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class ScraperService
{
    private readonly IActorProvider _actorProvider;
    private readonly ILogger<ScraperService> _logger;

    // Inject the specific provider and logger via Dependency Injection
    public ScraperService(IActorProvider actorProvider, ILogger<ScraperService> logger)
    {
        _actorProvider = actorProvider;
        _logger = logger;
    }

    // Method to scrape actors using the injected provider
    public async Task<List<Actor>> ScrapeActorsAsync()
    {
        try
        {
            // Attempt to get the actors from the provider
            return await _actorProvider.GetActorsAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An error occurred while scraping actors");

            // Handle the error by either returning an empty list or throwing an appropriate exception
            // Here we return an empty list to allow the application to continue running
            return new List<Actor>();
        }
    }
}
