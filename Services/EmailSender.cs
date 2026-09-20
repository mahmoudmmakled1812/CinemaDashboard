using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CinemaDashboard.Services;

public sealed class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = _configuration["Smtp:Host"];
        var port = _configuration.GetValue<int?>("Smtp:Port") ?? 587;
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];
        var fromEmail = _configuration["Smtp:FromEmail"] ?? username;
        var fromName = _configuration["Smtp:FromName"] ?? "Cinema Dashboard";
        var enableSsl = _configuration.GetValue("Smtp:EnableSsl", true);

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException(
                "SMTP is not configured. Set Smtp:Host, Smtp:Username, Smtp:Password and Smtp:FromEmail in configuration/user-secrets/environment variables.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(new MailAddress(email));

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, password),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        _logger.LogInformation("Sending email to {Email} with subject {Subject}", email, subject);
        await client.SendMailAsync(message);
    }
}
