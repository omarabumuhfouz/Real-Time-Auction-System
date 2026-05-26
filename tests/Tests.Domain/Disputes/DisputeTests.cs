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
    public void Open_ValidInputs_ReturnsDisputeWithDefaultOpenState()
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
        dispute.ResolvedAtUtc.ShouldBeNull();
        dispute.CreatedAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
    }

    [Fact]
    public void Open_WithNullImages_InitializesEmptyList()
    {
        // Arrange & Act
        var dispute = Dispute.Open(
            OrderId.New(), 
            DisputeTypeId.New(), 
            Title.Create("Title").Value, 
            Description.Create("Description").Value, 
            images: null); // Explicitly passing null

        // Assert
        dispute.Images.ShouldNotBeNull();
        dispute.Images.ShouldBeEmpty();
    }

    [Fact]
    public void Open_WhenCalled_RaisesDisputeOpenedDomainEvent()
    {
        // Arrange
        var orderId = OrderId.New();
        var disputeTypeId = DisputeTypeId.New();
        var title = Title.Create("Never received").Value;
        var description = Description.Create("I have been waiting for 3 weeks.").Value;

        // Act
        var dispute = Dispute.Open(orderId, disputeTypeId, title, description);

        // Assert
        dispute.ShouldNotBeNull();

        var domainEvents = dispute.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        
        var openedEvent = domainEvents.First().ShouldBeOfType<DisputeOpenedDomainEvent>();
        openedEvent.OrderId.ShouldBe(orderId);
        openedEvent.DisputeId.ShouldBe(dispute.Id);
    }

    #endregion

    #region State Mutations: UnderReview

    [Fact]
    public void UnderReview_StatusIsOpen_ChangesStatusToUnderReview()
    {
        // Arrange
        var dispute = CreateValidDispute();

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
        dispute.UnderReview(); 
        
        // Act
        var result = dispute.UnderReview();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Status.ShouldBe(DisputeStatus.UnderReview);
    }

    [Fact]
    public void UnderReview_StatusIsResolved_ReturnsFailureAlreadyResolved()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve(CreateValidResolution());

        // Act
        var result = dispute.UnderReview();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(DisputeErrors.AlreadyResolved); 
        dispute.Status.ShouldBe(DisputeStatus.Resolved);
    }

    #endregion

    #region State Mutations: Resolve

    [Fact]
    public void Resolve_StatusIsOpen_UpdatesResolutionAndRaisesEvent()
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
        dispute.Status.ShouldBe(DisputeStatus.Resolved);
        dispute.IsResolved.ShouldBeTrue();
        dispute.ResolvedAtUtc.ShouldNotBeNull();
        dispute.ResolvedAtUtc.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
        
        var domainEvents = dispute.DomainEvents;
        domainEvents.Count.ShouldBe(1);
        
        var resolvedEvent = domainEvents.First().ShouldBeOfType<DisputeResolvedDomainEvent>();
        resolvedEvent.DisputeId.ShouldBe(dispute.Id);
        resolvedEvent.OrderId.ShouldBe(dispute.OrderId);
        resolvedEvent.Resolution.ShouldBe(resolution.Value);
    }

    [Fact]
    public void Resolve_WhenStatusIsUnderReview_UpdatesStatusAndRaisesEvent()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.UnderReview();
        dispute.ClearDomainEvents();
        var resolution = CreateValidResolution();

        // Act
        var result = dispute.Resolve(resolution);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.Status.ShouldBe(DisputeStatus.Resolved);
        dispute.Resolution.ShouldBe(resolution);
        dispute.DomainEvents.First().ShouldBeOfType<DisputeResolvedDomainEvent>();
    }

    [Fact]
    public void Resolve_StatusIsAlreadyResolved_ReturnsSuccessWithoutRaisingEvent()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve(CreateValidResolution());
        dispute.ClearDomainEvents();
        
        var newResolution = Resolution.Create("Attempting new resolution.").Value;

        // Act
        var result = dispute.Resolve(newResolution);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        dispute.DomainEvents.ShouldBeEmpty(); // Event should not fire again
        
        // Note: Because it returns early with Result.Success(), the new resolution value is technically ignored 
        // by the domain. This assertion proves the state remains untouched.
        dispute.Resolution.Value.ShouldNotBe(newResolution.Value); 
    }

    #endregion

    #region Core Factory Helpers

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

    private static Resolution CreateValidResolution() => Resolution.Create("Testing Testing Testin").Value;

    #endregion
}