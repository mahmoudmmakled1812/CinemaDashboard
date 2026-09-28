using CinemaDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Data;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Movies.AnyAsync()) return;
        var action = new Category { Name = "Action" }; var drama = new Category { Name = "Drama" }; var comedy = new Category { Name = "Comedy" };
        var cinemas = new[]
        {
            new Cinema { Name = "Nile Cinema - Zamalek", ImageUrl = "https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?auto=format&fit=crop&w=1200&q=80" },
            new Cinema { Name = "City Stars Cinema", ImageUrl = "https://images.unsplash.com/photo-1517604931442-7e0c8ed2963c?auto=format&fit=crop&w=1200&q=80" },
            new Cinema { Name = "Downtown Screen", ImageUrl = "https://images.unsplash.com/photo-1485846234645-a62644f84728?auto=format&fit=crop&w=1200&q=80" }
        };
        var actors = new[]
        {
            new Actor { Name = "Omar Hassan", Bio = "Egyptian film actor.", ImageUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=600&q=80" },
            new Actor { Name = "Lina Adel", Bio = "Award-winning dramatic actress.", ImageUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80" },
            new Actor { Name = "Karim Samy", Bio = "Comedy and action performer.", ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80" },
            new Actor { Name = "Mariam Tarek", Bio = "Cinema and television actress.", ImageUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80" }
        };
        db.AddRange(action, drama, comedy); db.Cinemas.AddRange(cinemas); db.Actors.AddRange(actors); await db.SaveChangesAsync();
        var now = DateTime.Now;
        var movies = new[]
        {
            new Movie { Name = "Midnight in Cairo", Description = "A suspenseful night that changes the lives of three friends in Cairo.", Price = 180, ShowDateTime = now.AddDays(2).Date.AddHours(20), CategoryId = action.Id, CinemaId = cinemas[0].Id, MainImageUrl = "https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?auto=format&fit=crop&w=900&q=80" },
            new Movie { Name = "The Last Scene", Description = "An emotional drama about family, memory and second chances.", Price = 150, ShowDateTime = now.AddDays(3).Date.AddHours(18), CategoryId = drama.Id, CinemaId = cinemas[1].Id, MainImageUrl = "https://images.unsplash.com/photo-1485846234645-a62644f84728?auto=format&fit=crop&w=900&q=80" },
            new Movie { Name = "Weekend Escape", Description = "A light comedy where an unexpected trip turns into a big adventure.", Price = 140, ShowDateTime = now.AddDays(4).Date.AddHours(21), CategoryId = comedy.Id, CinemaId = cinemas[2].Id, MainImageUrl = "https://images.unsplash.com/photo-1517604931442-7e0c8ed2963c?auto=format&fit=crop&w=900&q=80" }
        };
        movies[0].MovieActors = [new MovieActor { ActorId = actors[0].Id }, new MovieActor { ActorId = actors[1].Id }];
        movies[1].MovieActors = [new MovieActor { ActorId = actors[1].Id }, new MovieActor { ActorId = actors[3].Id }];
        movies[2].MovieActors = [new MovieActor { ActorId = actors[0].Id }, new MovieActor { ActorId = actors[2].Id }];
        db.Movies.AddRange(movies); await db.SaveChangesAsync();
    }
}
