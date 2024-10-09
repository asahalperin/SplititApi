using HtmlAgilityPack;
using SplititScraperApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class RottenTomatoesActorProvider : IActorProvider
{
    private const string RotenTomatosUrl = "https://editorial.rottentomatoes.com/guide/fan-favorite-actors-2021/";

    public async Task<List<Actor>> GetActorsAsync()
    {
        var actors = new List<Actor>();
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(RotenTomatosUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Use XPath to retrieve all actor names
        var nodes = doc.DocumentNode.SelectNodes("//a[@class=\"celebrity\"]");
        // Use XPath to retrieve all actor Bio
        var bioNodes = doc.DocumentNode.SelectNodes("//*[@id='__next']//div[3]/div[normalize-space(text())]");

        // Iterate over each node and extract actor names
        int rank = 1;
        foreach (var node in nodes)
        {
            // Extract the actor's name from the current node
            var name = node.InnerText.Trim();

            // Create an Actor object and add it to the list
            actors.Add(new Actor
            {
                Id = rank,
                Name = name,
                Rank = rank,
                Bio = "IMDb bio placeholder"
            });

            rank++;
        }

        return actors;
    }
}
