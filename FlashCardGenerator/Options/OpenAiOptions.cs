namespace FlashCardGenerator.Options;

/// <summary>
/// Represents the settings specific to the OpenAI service.
/// </summary>
public class OpenAiOptions
{
  /// <summary>
  /// Gets the API key for accessing OpenAI services.
  /// </summary>
  /// <value>
  /// The API key used for authenticating requests to OpenAI.
  /// </value>
  public required string ApiKey { get; init; }
}