using Microsoft.EntityFrameworkCore;
using SplititScraperApi.Models;
using System.Collections.Generic;

public class ActorContext : DbContext
{
    public DbSet<Actor> Actors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("ActorDb");
    }
}
