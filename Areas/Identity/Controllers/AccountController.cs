using System.Security.Cryptography;
using CinemaDashboard.Constants;
using CinemaDashboard.Models;
using CinemaDashboard.Services;
using CinemaDashboard.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaDashboard.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private const string ResetUserIdKey = "PasswordResetUserId";
    private const string ResetTokenKey = "PasswordResetToken";
    private const string OtpVerifiedKey = "PasswordResetOtpVerified";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<ApplicationUserOTP> _otpRepository;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        IRepository<ApplicationUserOTP> otpRepository,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _otpRepository = otpRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid)
            return View(registerVM);

        var user = new ApplicationUser
        {
            FirstName = registerVM.FirstName.Trim(),
            LastName = registerVM.LastName.Trim(),
            Email = registerVM.Email.Trim(),
            UserName = registerVM.UserName.Trim(),
            Address = registerVM.Address?.Trim() ?? string.Empty
        };

        var result = await _userManager.CreateAsync(user, registerVM.Password);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(registerVM);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.CUSTOMER);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            AddIdentityErrors(roleResult);
            return View(registerVM);
        }

        try
        {
            await SendConfirmationEmailAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send confirmation email to {Email}", user.Email);
            TempData[NotificationConstants.WARNING_NOTIFICATION] =
                "Account created, but the confirmation email could not be sent. Please use Resend confirmation.";
            return RedirectToAction(nameof(ResendConfirmation));
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] =
            "Account created successfully. Please check your email to verify your account.";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> Confirm(string id, string token)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token))
            return BadRequest();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Invalid confirmation link.";
            return RedirectToAction(nameof(Login));
        }

        if (user.EmailConfirmed)
        {
            TempData[NotificationConstants.INFORMATION_NOTIFICATION] = "Your account is already verified.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] =
                "The confirmation link is invalid or expired. Please request a new one.";
        }
        else
        {
            TempData[NotificationConstants.SUCCESS_NOTIFICATION] =
                "Your account has been verified successfully. You can now log in.";
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid)
            return View(loginVM);

        var value = loginVM.EmailOrUserName.Trim();

        var user = await _userManager.FindByEmailAsync(value)
                   ?? await _userManager.FindByNameAsync(value);

        if (user is null)
        {
            AddInvalidLoginError();
            return View(loginVM);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user,
            loginVM.Password,
            loginVM.Remember,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError(
                nameof(LoginVM.EmailOrUserName),
                "Your account is temporarily locked because of too many failed attempts. Please try again later.");
            return View(loginVM);
        }

        if (signInResult.IsNotAllowed)
        {
            ModelState.AddModelError(
                nameof(LoginVM.EmailOrUserName),
                "Please verify your email address before logging in.");
            return View(loginVM);
        }

        if (!signInResult.Succeeded)
        {
            AddInvalidLoginError();
            return View(loginVM);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] =
            $"Welcome back, {user.FirstName} {user.LastName}!";

        if (await _userManager.IsInRoleAsync(user, RoleConstants.SUPER_ADMIN) ||
            await _userManager.IsInRoleAsync(user, RoleConstants.ADMIN))
        {
            return RedirectToAction(
                "Index",
                "Home",
                new { area = AreaConstants.ADMIN_AREA });
        }

        return RedirectToHome();
    }

    [HttpGet]
    public IActionResult ResendConfirmation()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmation(
        ResendEmailConfirmationVM resendEmailConfirmationVM)
    {
        if (!ModelState.IsValid)
            return View(resendEmailConfirmationVM);

        var value = resendEmailConfirmationVM.EmailOrUserName.Trim();

        var user = await _userManager.FindByEmailAsync(value)
                   ?? await _userManager.FindByNameAsync(value);

        if (user is not null && !user.EmailConfirmed)
        {
            try
            {
                await SendConfirmationEmailAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not resend confirmation email to {Email}", user.Email);
            }
        }

        // Do not reveal whether the account exists.
        TempData[NotificationConstants.INFORMATION_NOTIFICATION] =
            "If the account exists and is not verified, a new confirmation email has been sent.";

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "You have been logged out successfully.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(
        ForgetPasswordVM forgetPasswordVM,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(forgetPasswordVM);

        var value = forgetPasswordVM.EmailOrUserName.Trim();

        var user = await _userManager.FindByEmailAsync(value)
                   ?? await _userManager.FindByNameAsync(value);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
        {
            TempData[NotificationConstants.INFORMATION_NOTIFICATION] =
                "If an account matches the provided information, you will receive a password reset code.";
            return RedirectToAction(nameof(Login));
        }

        // Invalidate previous unused OTPs for this user.
        var previousOtps = _otpRepository
            .Get(o => o.ApplicationUserId == user.Id && !o.IsUsed)
            .ToList();

        foreach (var previousOtp in previousOtps)
            previousOtp.IsUsed = true;

        var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        await _otpRepository.CreateAsync(new ApplicationUserOTP
        {
            ApplicationUserId = user.Id,
            OTP = otp,
            CreateAt = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        }, ct);

        await _otpRepository.CommitAsync(ct);

        try
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Cinema Dashboard - Password Reset Code",
                $"<h2>Password reset</h2><p>Your verification code is:</p><h1>{otp}</h1><p>This code expires in 10 minutes.</p><p>If you did not request this, you can safely ignore this email.</p>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send password reset email to {Email}", user.Email);
            TempData[NotificationConstants.ERROR_NOTIFICATION] =
                "We could not send the reset email. Please try again later.";
            return RedirectToAction(nameof(Login));
        }

        TempData[ResetUserIdKey] = user.Id;
        TempData[NotificationConstants.INFORMATION_NOTIFICATION] =
            "A verification code has been sent to your email.";

        return RedirectToAction(nameof(ValidateOTP));
    }

    [HttpGet]
    public IActionResult ValidateOTP()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();

        if (TempData.Peek(ResetUserIdKey) is null)
            return RedirectToAction(nameof(ForgetPassword));

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidateOTP(
        ValidateOTPVM validateOTP,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(validateOTP);

        var userId = TempData.Peek(ResetUserIdKey)?.ToString();

        if (string.IsNullOrWhiteSpace(userId))
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] =
                "Your password reset session has expired. Please request a new code.";
            return RedirectToAction(nameof(ForgetPassword));
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Invalid password reset request.";
            return RedirectToAction(nameof(ForgetPassword));
        }

        var otpValue = validateOTP.OTP.Trim();

        var otpInDb = _otpRepository
            .Get(o =>
                o.ApplicationUserId == userId &&
                !o.IsUsed &&
                o.ValidTo >= DateTime.UtcNow)
            .OrderByDescending(o => o.CreateAt)
            .FirstOrDefault();

        if (otpInDb is null || !CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(otpValue),
                System.Text.Encoding.UTF8.GetBytes(otpInDb.OTP)))
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] =
                "The verification code is invalid or expired.";
            return RedirectToAction(nameof(ValidateOTP));
        }

        otpInDb.IsUsed = true;
        await _otpRepository.CommitAsync(ct);

        // Generate the Identity reset token only after the OTP has been verified.
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        TempData[ResetUserIdKey] = user.Id;
        TempData[ResetTokenKey] = resetToken;
        TempData[OtpVerifiedKey] = true;

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] =
            "Code verified. You can now choose a new password.";

        return RedirectToAction(nameof(NewPassword));
    }

    [HttpGet]
    public IActionResult NewPassword()
    {
        if (TempData.Peek(ResetUserIdKey) is null ||
            TempData.Peek(ResetTokenKey) is null ||
            TempData.Peek(OtpVerifiedKey) is null)
        {
            return RedirectToAction(nameof(ForgetPassword));
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewPassword(
        NewPasswordVM newPasswordVM)
    {
        if (!ModelState.IsValid)
            return View(newPasswordVM);

        var userId = TempData.Peek(ResetUserIdKey)?.ToString();
        var resetToken = TempData.Peek(ResetTokenKey)?.ToString();
        var otpVerified = TempData.Peek(OtpVerifiedKey)?.ToString();

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(resetToken) ||
            otpVerified != bool.TrueString)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] =
                "Your password reset session has expired. Please request a new code.";
            return RedirectToAction(nameof(ForgetPassword));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return RedirectToAction(nameof(ForgetPassword));

        var result = await _userManager.ResetPasswordAsync(
            user,
            resetToken,
            newPasswordVM.Password);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(newPasswordVM);
        }

        TempData.Remove(ResetUserIdKey);
        TempData.Remove(ResetTokenKey);
        TempData.Remove(OtpVerifiedKey);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] =
            "Your password has been changed successfully. Please log in.";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private async Task SendConfirmationEmailAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = Url.Action(
            nameof(Confirm),
            "Account",
            new
            {
                area = AreaConstants.IDENTITY_AREA,
                id = user.Id,
                token
            },
            Request.Scheme);

        if (string.IsNullOrWhiteSpace(link))
            throw new InvalidOperationException("Could not generate the email confirmation link.");

        var body = $"""
            <h2>Welcome to Cinema Dashboard</h2>
            <p>Hello {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>
            <p>Please confirm your email address by clicking the button below:</p>
            <p><a href="{link}" style="display:inline-block;padding:10px 18px;background:#2c7be5;color:#fff;text-decoration:none;border-radius:5px;">Confirm my email</a></p>
            <p>If you did not create this account, you can ignore this email.</p>
            """;

        await _emailSender.SendEmailAsync(
            user.Email!,
            "Cinema Dashboard - Confirm your account",
            body);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }

    private void AddInvalidLoginError()
    {
        ModelState.AddModelError(
            nameof(LoginVM.EmailOrUserName),
            "Invalid username/email or password.");

        ModelState.AddModelError(
            nameof(LoginVM.Password),
            "Invalid username/email or password.");
    }

    private IActionResult RedirectToHome()
    {
        return RedirectToAction(
            "Index",
            "Home",
            new { area = AreaConstants.CUSTOMER_AREA });
    }
}
