namespace MazadZone.Domain.Disputes;
public class DisputeTypeErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "DisputeType.NotFound",
        "The dispute type was not found.");
}