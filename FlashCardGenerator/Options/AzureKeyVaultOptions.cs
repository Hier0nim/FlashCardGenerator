using System.ComponentModel.DataAnnotations;

namespace FlashCardGenerator.Options;

/// <summary>
/// Represents the settings required to configure access to Azure Key Vault.
/// </summary>
public class AzureKeyVaultOptions
{
    /// <summary>
    /// Gets or sets the URI of the Azure Key Vault. This URI is required and must be a valid Azure Key Vault endpoint.
    /// </summary>
    /// <value>
    /// The URI should follow the format: "https://"your-key-vault-name".vault.azure.net/".
    /// </value>
    [Required]
    public required string Uri { get; set; }
}