using CinemaDashboard.Models;

namespace CinemaDashboard.ViewModels;

public class CustomerHomeVM
{
    public List<Movie> Movies { get; init; } = [];
    public List<Cinema> Cinemas { get; init; } = [];
    public List<Actor> Actors { get; init; } = [];
}
