using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    [Required, StringLength(40)] public string Method { get; set; } = "Demo card";
    [Required, StringLength(40)] public string Status { get; set; } = "Paid";
    [Required, StringLength(80)] public string TransactionReference { get; set; } = string.Empty;
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
    public Order? Order { get; set; }
}
