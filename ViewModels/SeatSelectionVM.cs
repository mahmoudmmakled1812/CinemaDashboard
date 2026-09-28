using CinemaDashboard.Models;

namespace CinemaDashboard.ViewModels;

public class SeatSelectionVM
{
    public required Movie Movie { get; init; }
    public HashSet<string> ReservedSeats { get; init; } = [];
}
