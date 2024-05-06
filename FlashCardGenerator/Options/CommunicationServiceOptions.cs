using System.ComponentModel.DataAnnotations;

namespace FlashCardGenerator.Options;

/// <summary>
/// Represents the configuration settings for communication services used within the application.
/// </summary>
public class CommunicationServiceOptions
{
    /// <summary>
    /// Gets or sets the connection string for the communication service. This must be a valid URI to the service endpoint.
    /// </summary>
    /// <value>
    /// The connection string is required and should follow the format: "https://"your-communication-service"."region".communication.azure.com".
    /// </value>
    [Required]
    public required string ConnectionString { get; set; }
}