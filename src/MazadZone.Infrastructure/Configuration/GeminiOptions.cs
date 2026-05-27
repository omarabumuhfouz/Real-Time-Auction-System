namespace MazadZone.Infrastructure.Configuration;

/// <summary>
/// Configuration options for the Google Gemini AI service.
/// API key must be provided via environment variables or .NET User Secrets — never hardcoded.
/// </summary>
public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    /// <summary>
    /// The Gemini API key. Must be supplied via environment variable or user secrets.
    /// Do NOT set this in appsettings.json for production deployments.
    /// </summary>
    public string ApiKey { get; init; } = "AIzaSyBvD17yw138lAhp7S6m7pVRmsk_JXLJC2U";

    /// <summary>
    /// The Gemini model to use (e.g., "gemini-2.0-flash", "gemini-2.5-pro").
    /// </summary>
    public string Model { get; init; } = "gemini-3.0-flash";

    /// <summary>
    /// Temperature for generation. Low values (0.1) reduce hallucination.
    /// </summary>
    public float Temperature { get; init; } = 0.1f;
}
