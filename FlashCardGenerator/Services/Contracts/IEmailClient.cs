namespace FlashCardGenerator.Services.Contracts;

/// <summary>
/// Defines the contract for an email client service.
/// </summary>
public interface IEmailClient
{
    /// <summary>
    /// Sends an email message asynchronously.
    /// </summary>
    /// <param name="toMailAddress">Recipient email address.</param>
    /// <param name="subject">Subject of the email.</param>
    /// <param name="htmlBody">HTML body content of the email.</param>
    /// <param name="plainTextContent">Optional plain text content of the email.</param>
    /// <param name="ct">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(string toMailAddress, string subject, string htmlBody, string? plainTextContent = null,
        CancellationToken ct = default);
}