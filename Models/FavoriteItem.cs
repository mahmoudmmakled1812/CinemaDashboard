using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class FavoriteItem
{
    public int Id { get; set; }
    [Required] public string ApplicationUserId { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ApplicationUser? ApplicationUser { get; set; }
    public Movie? Movie { get; set; }
}
