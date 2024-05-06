using System.ComponentModel.DataAnnotations;

namespace FlashCardGenerator.Options;

/// <summary>
/// Represents the configuration settings for email operations within the application.
/// </summary>
public class EMailOptions
{
    /// <summary>
    /// Gets or sets the domain name part of an email address which is required and must be a valid email address format.
    /// </summary>
    /// <example>example.com</example>
    [Required]
    [EmailAddress]
    public required string DomainName { get; set; }
}