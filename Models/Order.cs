using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class Order
{
    public int Id { get; set; }
    [Required] public string ApplicationUserId { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Confirmed;
    public ApplicationUser? ApplicationUser { get; set; }
    public Movie? Movie { get; set; }
    public ICollection<BookingSeat> Seats { get; set; } = [];
    public Payment? Payment { get; set; }
}
