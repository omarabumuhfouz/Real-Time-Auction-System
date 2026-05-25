namespace MazadZone.Domain.Shared.Errors;

public static class TitleErrors
{
    public static readonly Error Empty = Error.Validation(
        "Title.Empty",
        "The title cannot be empty.");

    public static readonly Error TooLong = Error.Validation(
        "Title.TooLong",
        $"The title cannot be longer than {ValueObjects.Title.MaxLength} characters.");
}