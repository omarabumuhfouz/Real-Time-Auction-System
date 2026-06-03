using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Tesseract;
using MazadZone.Application.Services;

namespace MazadZone.Infrastructure.Services;

public class TesseractIdentityExtractionService : IIdentityExtractionService
{
    public async Task<ExtractedIdData> ExtractDataAsync(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            return new ExtractedIdData(null, false, "Image data is empty or null.");
        }

        byte[] preprocessedBytes;

        // 1. IMPROVED IMAGE PRE-PROCESSING
        try
        {
            using var image = Image.Load<Rgba32>(imageBytes);
            image.Mutate(ctx => ctx
                // Upscale image to improve readability
                .Resize(new ResizeOptions { Size = new Size(1600, 0), Mode = ResizeMode.Max })
                .Grayscale()
                .Contrast(1.2f));

            using var ms = new MemoryStream();
            await image.SaveAsPngAsync(ms);
            preprocessedBytes = ms.ToArray();
        }
        catch (Exception ex)
        {
            return new ExtractedIdData(null, false, $"Image processing failed: {ex.Message}");
        }

        // 2. OCR EXTRACTION
        string rawText;
        var tessdataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

        try
        {
            using var engine = new TesseractEngine(tessdataDir, "ara+eng", EngineMode.Default);
            // SparseText works better for ID cards where text is scattered
            engine.DefaultPageSegMode = PageSegMode.SparseText;
            using var pix = Pix.LoadFromMemory(preprocessedBytes);
            using var page = engine.Process(pix);
            rawText = page.GetText();
        }
        catch (Exception ex)
        {
            return new ExtractedIdData(null, false, $"OCR Engine failure: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(rawText))
        {
            return new ExtractedIdData(null, false, "No text detected.");
        }

        // 3. ADVANCED REGEX PARSING
        string? nationalId = null;
        string? englishFullName = null;
        string? arabicFullName = null;
        string? dateOfBirth = null;
        string? gender = null;

        // Initial cleanup to remove hidden Unicode markers
        var cleanText = rawText.Replace("\u200E", "").Replace("\u200F", "");

        // National ID: Remove all whitespaces first to ensure we capture the number even if it's read with gaps
        var textWithoutSpaces = Regex.Replace(cleanText, @"\s+", "");
        var idMatch = Regex.Match(textWithoutSpaces, @"(19\d{8}|20\d{8}|9\d{9})");
        if (idMatch.Success) nationalId = idMatch.Value;

        // English Name: Ignore excessive whitespaces
        var engNameMatch = Regex.Match(cleanText, @"Name:\s*([A-Z\s]+)");
        if (engNameMatch.Success) englishFullName = engNameMatch.Groups[1].Value.Trim();

        // Arabic Name: Reduce reliance on the exact word 'Name' to avoid OCR errors
        var arNameMatch = Regex.Match(cleanText, @"(?:الاسم|الاسـم)[:\s]*(.+)");
        if (arNameMatch.Success) 
        {
            arabicFullName = arNameMatch.Groups[1].Value.Trim();
        }
        else 
        {
            // Fallback: Find 3 to 4 consecutive Arabic words (likely the full name)
            // But ignore lines containing common ID card headers
            var restrictedWords = new[] { "بطاقة", "شخصية", "دائرة", "أحوال", "الأحوال", "المدنية", "الجوازات", "المملكة", "الاردنية", "الأردنية", "الهاشمية", "وزارة", "الداخلية", "رقم", "وطني" };
            var lines = cleanText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (restrictedWords.Any(w => line.Contains(w)))
                    continue;

                var fallbackMatch = Regex.Match(line, @"([\u0600-\u06FF]{2,}\s+[\u0600-\u06FF]{2,}\s+[\u0600-\u06FF]{2,}(?:\s+[\u0600-\u06FF]{2,})?)");
                if (fallbackMatch.Success)
                {
                    arabicFullName = fallbackMatch.Groups[1].Value.Trim();
                    break;
                }
            }
        }

        // Date of Birth: Support multiple formats (10 /02/2004 or 10-02-2004)
        var dobMatch = Regex.Match(cleanText, @"(\d{2}[\/\-]\d{2}[\/\-]\d{4})");
        if (dobMatch.Success) dateOfBirth = dobMatch.Groups[1].Value;

        // Gender: Capture M or F and ignore surrounding incorrect symbols
        var genderMatch = Regex.Match(cleanText, @"([MF])(?:[\/\\]|$)");
        if (genderMatch.Success) gender = genderMatch.Groups[1].Value;

        // 4. VALIDATION
        bool success = true;
        string? errorMessage = null;

        if (string.IsNullOrEmpty(nationalId))
        {
            success = false;
            errorMessage = $"Could not extract core data. Debug Output: \n{rawText}";
        }

        return new ExtractedIdData(
            NationalId: nationalId,
            Success: success,
            ErrorMessage: errorMessage,
            EnglishFullName: englishFullName,
            ArabicFullName: arabicFullName,
            DateOfBirth: dateOfBirth,
            Gender: gender);
    }
}