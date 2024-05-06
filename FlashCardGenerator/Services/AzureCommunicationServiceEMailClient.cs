using Azure.Communication.Email;
using FlashCardGenerator.Options;
using FlashCardGenerator.Services.Contracts;
using Microsoft.Extensions.Options;

namespace FlashCardGenerator.Services
{
    /// <summary>
    /// Provides email sending capabilities via Azure Communication Services.
    /// </summary>
    public class AzureCommunicationServiceEmailClient : IEmailClient
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
            _logger = logger;
            _mailClient = new EmailClient(acsOptions.Value.ConnectionString);
        }

        /// <summary>
        /// Sends an email message asynchronously using Azure Communication Services.
        /// </summary>
        /// <param name="toMailAddress">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="htmlBody">HTML body content of the email.</param>
        /// <param name="plainTextContent">Optional plain text content.</param>
        /// <param name="ct">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(string toMailAddress, string subject, string htmlBody,
            string? plainTextContent = null, CancellationToken ct = default)
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