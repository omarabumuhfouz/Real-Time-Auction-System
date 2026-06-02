using Google.Cloud.Vision.V1;
using MazadZone.Application.Services;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace MazadZone.Infrastructure.Services;

public class GoogleVisionIdentityExtractionService : IIdentityExtractionService
{
    private readonly IServiceProvider _serviceProvider;
    
    // Target 10 to 15 digit national identifier sequence.
    // Specifying a 250ms matching timeout to prevent resource-exhaustion/ReDoS attacks.
    private static readonly Regex NationalIdRegex = new Regex(
        @"\d{10,15}", 
        RegexOptions.Compiled, 
        TimeSpan.FromMilliseconds(250));

    public GoogleVisionIdentityExtractionService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ExtractedIdData> ExtractDataAsync(byte[] imageBytes)
    {
        try
        {
            // Resolve the client inside the try-catch block to handle missing credentials gracefully
            ImageAnnotatorClient client;
            try
            {
                client = _serviceProvider.GetRequiredService<ImageAnnotatorClient>();
            }
            catch (Exception ex)
            {
                return new ExtractedIdData(null, false, "Google Cloud Vision API credentials are not configured on this server: " + (ex.InnerException?.Message ?? ex.Message));
            }

            var image = Image.FromBytes(imageBytes);
            
            // Invoke DetectDocumentTextAsync for dense document text/layout optimization mode
            var response = await client.DetectDocumentTextAsync(image);
            
            var rawText = response?.Text;
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return new ExtractedIdData(null, false, "No text was detected on the ID card. Please ensure the image is clear and try again.");
            }

            // Strip whitespaces, newlines, and hyphens to build a contiguous raw string
            var cleanText = rawText
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("\r", "")
                .Replace("\n", "");

            var match = NationalIdRegex.Match(cleanText);
            if (match.Success)
            {
                return new ExtractedIdData(match.Value, true, null);
            }

            return new ExtractedIdData(null, false, "Could not locate a valid National ID sequence in the document text.");
        }
        catch (Grpc.Core.RpcException ex)
        {
            // Intercept specific SDK faults or network drops, presenting a clean validation error message
            return new ExtractedIdData(null, false, $"Network or API connection error with identity recognition service: {ex.Status.Detail}");
        }
        catch (Exception ex)
        {
            return new ExtractedIdData(null, false, $"An unexpected error occurred during OCR text extraction: {ex.Message}");
        }
    }
}
