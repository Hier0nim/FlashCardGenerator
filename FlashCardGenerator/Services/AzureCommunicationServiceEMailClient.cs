using Azure.Communication.Email;
using FlashCardGenerator.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace FlashCardGenerator.Services
{
    /// <summary>
    /// Provides email sending capabilities via Azure Communication Services.
    /// </summary>
    public class AzureCommunicationServiceEmailClient : IEmailSender
    {
        private readonly EmailClient _mailClient;
        private readonly EMailOptions _emailOptions;
        private readonly ILogger<AzureCommunicationServiceEmailClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCommunicationServiceEmailClient"/> class.
        /// </summary>
        /// <param name="emailOptions">The email options configuration.</param>
        /// <param name="acsOptions">The communication service options configuration.</param>
        /// <param name="logger">The logger instance.</param>
        public AzureCommunicationServiceEmailClient(
            IOptions<EMailOptions> emailOptions,
            IOptions<CommunicationServiceOptions> acsOptions,
            ILogger<AzureCommunicationServiceEmailClient> logger)
        {
            _emailOptions = emailOptions.Value;
            _mailClient = new EmailClient(acsOptions.Value.ConnectionString);
            _logger = logger;
        }

        /// <summary>
        /// Sends an email message asynchronously using Azure Communication Services.
        /// </summary>
        /// <param name="email">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="htmlMessage">HTML body content of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendEmailAsync(email, subject, htmlMessage, null);
        }

        /// <summary>
        /// Sends an email message asynchronously with support for HTML and plain text content.
        /// </summary>
        /// <param name="toMailAddress">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="htmlBody">HTML body content of the email.</param>
        /// <param name="plainTextContent">Optional plain text content.</param>
        /// <param name="ct">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SendEmailAsync(string toMailAddress, string subject, string htmlBody,
            string? plainTextContent, CancellationToken ct = default)
        {
            var fromAddress = _emailOptions.DomainName;
            try
            {
                var response = await _mailClient.SendAsync(
                    Azure.WaitUntil.Completed,
                    fromAddress,
                    toMailAddress,
                    subject,
                    htmlBody,
                    plainTextContent,
                    ct);

                _logger.LogInformation(response.Value.Status.ToString() switch
                {
                    "Succeeded" => $"Email to {toMailAddress} queued successfully!",
                    _ => $"Failure in sending email to {toMailAddress}: {response.Value.Status.ToString()}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toMailAddress}: {ex.Message}");
                throw;
            }
        }
    }
}
