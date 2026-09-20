using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.ViewModels;

public class ValidateOTPVM
{
    [Required(ErrorMessage = "Please enter the verification code.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "The verification code must be 6 digits.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "The verification code must contain 6 digits.")]
    [Display(Name = "Verification Code")]
    public string OTP { get; set; } = string.Empty;
}
