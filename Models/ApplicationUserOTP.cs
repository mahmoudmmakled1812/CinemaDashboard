namespace CinemaDashboard.Models;

public class ApplicationUserOTP
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? ApplicationUser { get; set; }
    public string OTP { get; set; } = string.Empty;
    public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddMinutes(10);
    public bool IsUsed { get; set; } = false;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}