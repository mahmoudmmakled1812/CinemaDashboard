using CinemaDashboard.Models;

namespace CinemaDashboard.ViewModels;

public class AdminDashboardVM
{
    public int Movies { get; init; }
    public int Cinemas { get; init; }
    public int Customers { get; init; }
    public int ConfirmedOrders { get; init; }
    public decimal Revenue { get; init; }
    public List<Order> RecentOrders { get; init; } = [];
}
