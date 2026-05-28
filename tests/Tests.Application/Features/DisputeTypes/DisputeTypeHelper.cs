namespace Tests.Application.Features.DisputeTypes;

using MazadZone.Features.DisputeTypes.Commands.Create;

public static class DisputeTypeHelper
{
    public static CreateDisputeTypeCommand CreateValidCreateCommand()
    {
        return new CreateDisputeTypeCommand(
            "Item Not Received", 
            "The buyer did not receive the item after payment.");
    }

    public static CreateDisputeTypeCommand CreateInvalidCreateCommand()
    {
        // Assuming an empty name triggers a domain validation failure
        return new CreateDisputeTypeCommand(
            string.Empty, 
            "This is an invalid dispute type because the name is empty.");
    }
}