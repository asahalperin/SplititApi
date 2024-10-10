using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using SplititScraperApi.Models;


[ApiController]
[Route("api/v1/[controller]")]
public class ActorsController : ControllerBase
{
    private readonly ScraperService _scraperService;
    private readonly TokenStorage _tokenStorage;  // Inject TokenStorage


    // Separate list for manually added actors
    private static List<Actor> _manuallyAddedActors = new List<Actor>();

    // Main list for actors (scraped + manually added)
    private static List<Actor> _actors = new List<Actor>();

    //private static string token = "Bearer 1234567890";

    public ActorsController(ScraperService scraperService, TokenStorage tokenStorage)
    {
        _scraperService = scraperService;
        _tokenStorage = tokenStorage;  // Initialize the token storage
    }

    // GET: api/v1/actors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> ScrapeActors(
        [FromQuery] string actorName = null,
        [FromQuery] int? minRank = null,
        [FromQuery] int? maxRank = null,
        [FromHeader] int? skip = null,
        [FromHeader] int? take = null,
        [FromHeader] string Authorization = null
    )
    {
        if (Authorization == null || !Authorization.Equals($"Bearer {_tokenStorage.GeneratedToken}"))
        {
            return Unauthorized("Invalid or missing token.");
        }
        // Scrape actors from the service
        var scrapedActors = await _scraperService.ScrapeActorsAsync();

        // Clear the main actors list and add newly scraped actors
        _actors = new List<Actor>(scrapedActors);

        // Add manually added actors back to the main list
        _actors.AddRange(_manuallyAddedActors);

        // Filtering by actorName
        var filteredActors = _actors;

        if (!string.IsNullOrEmpty(actorName))
        {
            filteredActors = filteredActors.Where(a => a.Name.Contains(actorName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Filtering by rank (as range)
        if (minRank.HasValue)
        {
            filteredActors = filteredActors.Where(a => a.Rank >= minRank.Value).ToList();
        }

        if (maxRank.HasValue)
        {
            filteredActors = filteredActors.Where(a => a.Rank <= maxRank.Value).ToList();
        }

        // Pagination: Apply pagination only if skip and take are provided
        if (skip.HasValue && take.HasValue)
        {
            var totalActors = filteredActors.Count;
            var totalPages = (int)Math.Ceiling(totalActors / (double)take.Value);
            var paginatedActors = filteredActors
                .Skip((skip.Value - 1) * take.Value)
                .Take(take.Value)
                .Select(a => new { a.Id, a.Name })
                .ToList();

            // Returning the paginated result with metadata
            return Ok(new
            {
                TotalActors = totalActors,
                TotalPages = totalPages,
                CurrentPage = skip.Value,
                PageSize = take.Value,
                Actors = paginatedActors
            });
        }

        // If no pagination parameters are provided, return the full list of actors
        return Ok(filteredActors.Select(a => new { a.Id, a.Name }).ToList());
    }

    // POST: api/v1/actors - Add a new actor
    [HttpPost]
    public ActionResult<Actor> AddActor([FromBody] Actor newActor)
    {
        if (_manuallyAddedActors.Any(a => a.Id == newActor.Id))
        {
            return Conflict("Actor with this ID already exists.");
        }
        if (_manuallyAddedActors.Any(a => a.Rank == newActor.Rank))
        {
            return Conflict("An actor with this rank already exists.");
        }

        // Add the new actor to the manually added actors list
        _manuallyAddedActors.Add(newActor);

        // Also add to the main actors list
        _actors.Add(newActor);

        return CreatedAtAction(nameof(GetActorById), new { id = newActor.Id }, newActor);
    }

    // GET: api/v1/actors/{id} - Get details of a specific actor
    [HttpGet("{id}")]
    public ActionResult<Actor> GetActorById(int id)
    {
        var actor = _actors.FirstOrDefault(a => a.Id == id);
        if (actor == null)
        {
            return NotFound("Actor not found.");
        }
        return Ok(actor); // Return full actor model
    }

    // PUT: api/v1/actors/{id} - Update an actor's details
    [HttpPut("{id}")]
    public ActionResult UpdateActor(int id, [FromBody] Actor updatedActor)
    {
        var actor = _actors.FirstOrDefault(a => a.Id == id);
        if (actor == null)
        {
            return NotFound("Actor not found.");
        }

        // Check for duplicate rank, but allow the same actor to keep their own rank
        if (_actors.Any(a => a.Rank == updatedActor.Rank && a.Id != id))
        {
            return Conflict("An actor with this rank already exists.");
        }

        // Update the actor's details in both lists
        actor.Name = updatedActor.Name;
        actor.Rank = updatedActor.Rank;
        actor.Bio = updatedActor.Bio;

        // Also update the actor in the manually added actors list if applicable
        var manualActor = _manuallyAddedActors.FirstOrDefault(a => a.Id == id);
        if (manualActor != null)
        {
            manualActor.Name = updatedActor.Name;
            manualActor.Rank = updatedActor.Rank;
            manualActor.Bio = updatedActor.Bio;
        }

        return NoContent(); // No content status to indicate a successful update
    }



    // DELETE: api/v1/actors/{id} - Delete an actor
    [HttpDelete("{id}")]
    public ActionResult DeleteActor(int id)
    {
        var actor = _actors.FirstOrDefault(a => a.Id == id);
        if (actor == null)
        {
            return NotFound("Actor not found.");
        }

        // Remove from both lists
        _actors.Remove(actor);
        _manuallyAddedActors.RemoveAll(a => a.Id == id);

        return NoContent();
    }
}

