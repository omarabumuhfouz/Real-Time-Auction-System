using MazadZone.Domain.Disputes;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Shared.ValueObjects;
using Shouldly;

namespace Tests.Domain.Disputes;

public class DisputeTests
{
    #region Factory: Create & Open

    [Fact]
    public void Create_ValidInputs_ReturnsDisputeWithDefaultOpenState()
    {
        // Arrange
        var orderId = OrderId.New();
        var disputeTypeId = DisputeTypeId.New();
        var title = Title.Create("Item not as described").Value;
        var description = Description.Create("The item has scratches not shown in pictures.").Value;
        var images = new List<Image>();

        // Act
        var dispute = Dispute.Open(orderId, disputeTypeId, title, description, images);

        // Assert
        
        dispute.OrderId.ShouldBe(orderId);
        dispute.DisputeTypeId.ShouldBe(disputeTypeId);
        dispute.Title.ShouldBe(title);
        dispute.Description.ShouldBe(description);
        dispute.Status.ShouldBe(DisputeStatus.Open);
        dispute.IsResolved.ShouldBeFalse();
        dispute.Resolution.ShouldBe(Resolution.Empty);
        dispute.Images.ShouldBeEmpty();
        dispute.CreatedAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
    }

    [Fact]
    public void Open_WhenCalled_RaisesDisputeOpenedDomainEvent()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.ClearDomainEvents();
        
        var orderId = OrderId.New();
        var disputeTypeId = DisputeTypeId.New();
        var title = Title.Create("Never received").Value;
        var description = Description.Create("I have been waiting for 3 weeks.").Value;

        // Act
       var result =   Dispute.Open(orderId, disputeTypeId, title, description);


        // Assert
        result.ShouldNotBeNull();

        var domainEvents = dispute.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        
        var openedEvent = domainEvents.First().ShouldBeOfType<DisputeOpenedDomainEvent>();
        openedEvent.OrderId.ShouldBe(orderId);
        // Note: based on the provided logic, it raises the event using the newly created temporary dispute ID inside Open()
    }

    #endregion

    #region State Mutations: UnderReview

    [Fact]
    public void UnderReview_StatusIsOpen_ChangesStatusToUnderReview()
    {
        // Arrange
        var dispute = CreateValidDispute(); // Default is Open

        // Act
        var result = dispute.UnderReview();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Status.ShouldBe(DisputeStatus.UnderReview);
    }

    [Fact]
    public void UnderReview_StatusIsAlreadyUnderReview_ReturnsSuccessAndKeepsStatus()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.UnderReview(); // Set to UnderReview

        // Act
        var result = dispute.UnderReview();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Status.ShouldBe(DisputeStatus.UnderReview); // Status remains unchanged
    }

    [Fact]
    public void UnderReview_StatusIsResolved_ReturnsSuccessAndKeepsStatus()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve("Refund issued to buyer."); // Set to Resolved

        // Act
        var result = dispute.UnderReview();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Status.ShouldBe(DisputeStatus.Resolved); // Should not change back to UnderReview
    }

    #endregion

    #region State Mutations: ChangeDescription

    [Fact]
    public void ChangeDescription_StatusIsOpen_UpdatesDescriptionAndReturnsSuccess()
    {
        // Arrange
        var dispute = CreateValidDispute();
        var newDescription = Description.Create("Updated description with more details.").Value;

        // Act
        var result = dispute.ChangeDescription(newDescription);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Description.ShouldBe(newDescription);
    }

    [Fact]
    public void ChangeDescription_StatusIsResolved_ReturnsFailure()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve("Resolved with refund");
        var newDescription = Description.Create("Trying to update after resolution.").Value;

        // Act
        var result = dispute.ChangeDescription(newDescription);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.DisputeCannotChangeReason);
    }

    #endregion

    #region State Mutations: Resolve

    [Fact]
    public void Resolve_WithResolutionVO_StatusIsNotResolved_UpdatesResolutionAndRaisesEvent()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.ClearDomainEvents();
        var resolution = Resolution.Create("Seller agreed to refund.").Value;

        // Act
        var result = dispute.Resolve(resolution);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Resolution.ShouldBe(resolution);
        
        var domainEvents = dispute.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        
        var resolvedEvent = domainEvents.First().ShouldBeOfType<DisputeResolvedDomainEvent>();
        resolvedEvent.DisputeId.ShouldBe(dispute.Id);
        resolvedEvent.OrderId.ShouldBe(dispute.OrderId);
        resolvedEvent.Resolution.ShouldBe(resolution.Value);
    }

    [Fact]
    public void Resolve_WithResolutionVO_StatusIsAlreadyResolved_ReturnsSuccessWithoutRaisingEvent()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve("Initial Resolution"); // Sets status to Resolved
        dispute.ClearDomainEvents();
        var newResolution = Resolution.Create("Attempting new resolution.").Value;

        // Act
        var result = dispute.Resolve(newResolution);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.DomainEvents.ShouldBeEmpty(); // Event should not fire again
    }

    [Fact]
    public void Resolve_WithString_StatusIsNotResolved_UpdatesStatusResolutionAndDate()
    {
        // Arrange
        var dispute = CreateValidDispute();
        var resolutionText = "Partial refund processed.";

        // Act
        var result = dispute.Resolve(resolutionText);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Status.ShouldBe(DisputeStatus.Resolved);
        dispute.IsResolved.ShouldBeTrue();
        dispute.Resolution.Value.ShouldBe(resolutionText);
        dispute.ResolvedAtUtc.ShouldNotBeNull();
        dispute.ResolvedAtUtc.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
    }

    [Fact]
    public void Resolve_WithString_StatusIsAlreadyResolved_ReturnsFailure()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve("First resolution");

        // Act
        var result = dispute.Resolve("Trying to resolve again.");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.DisputeAlreadyResolved);
    }

    #endregion

    // --- Core Factory Helpers ---

    private static Dispute CreateValidDispute()
    {
        return Dispute.Open(
            OrderId.New(),
            DisputeTypeId.New(),
            Title.Create("Valid Title").Value,
            Description.Create("Valid Description").Value,
            new List<Image>()
        );
    }
}