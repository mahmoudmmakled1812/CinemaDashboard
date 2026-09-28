using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class BookingSeat
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MovieId { get; set; }
    [Range(1, 30)] public int RowNumber { get; set; }
    [Range(1, 30)] public int SeatNumber { get; set; }
    public Order? Order { get; set; }
    public Movie? Movie { get; set; }
    public string Label => $"{(char)('A' + RowNumber - 1)}{SeatNumber}";
}
