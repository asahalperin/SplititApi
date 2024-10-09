using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using SplititScraperApi.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class IMDbActorProvider : IActorProvider
{
    private const string IMDbUrl = "https://www.imdb.com/list/ls054840033/";
    private readonly ILogger<IMDbActorProvider> _logger; // Logger for error logging

    // Inject logger via constructor
    public IMDbActorProvider(ILogger<IMDbActorProvider> logger)
    {
        _logger = logger;
    }

    public async Task<List<Actor>> GetActorsAsync()
    {
        var actors = new List<Actor>();

        try
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(IMDbUrl);

            // HtmlAgilityPack usage
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Use XPath to retrieve all actor names
            var nameNodes = doc.DocumentNode.SelectNodes("//*[@id='__next']//a/h3");
            // Use XPath to retrieve all actor Bio
            var bioNodes = doc.DocumentNode.SelectNodes("//*[@id='__next']//div[3]/div[normalize-space(text())]");

            // Regular expression pattern to match only letters
            var regex = new Regex("[^a-zA-Z]+");

            // Iterate over each node and extract actor names
            int rank = 1;
            //foreach (var node in nameNodes)
            for (int i = 0; i < nameNodes.Count; i++)
            {
                // Extract the actor's name from the current node
                var name = nameNodes[i].InnerText.Trim();
                name = regex.Replace(name, "", 1);

                // Extract the actor's bio from the current node
                var bio = bioNodes[i].InnerText.Trim();

                // Create an Actor object and add it to the list
                actors.Add(new Actor
                {
                    Id = rank,
                    Name = name,
                    Rank = rank,
                    Bio = bio
                });

                rank++;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error occurred while fetching data from IMDb.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
        }

        // Return the list of actors
        return actors;
    }
}
