using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.ViewModels;

public class NewPasswordVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
