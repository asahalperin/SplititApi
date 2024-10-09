using System.Collections.Generic;
using System.Threading.Tasks;
using SplititScraperApi.Models;

public interface IActorProvider
{
    Task<List<Actor>> GetActorsAsync();
}
