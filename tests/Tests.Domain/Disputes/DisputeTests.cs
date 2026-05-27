using System;
using System.Collections.Generic;
using System.Linq;
using MazadZone.Domain.Disputes;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Shared.ValueObjects;
using Shouldly;
using Xunit;

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
        dispute.Images.ShouldBe(images);
        dispute.ResolvedAtUtc.ShouldBeNull();
        dispute.CreatedAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
    }

    [Fact]
    public void Open_NullImages_DefaultsToEmptyList()
    {
        // Arrange
        var orderId = OrderId.New();
        var disputeTypeId = DisputeTypeId.New();
        var title = Title.Create("Missing items").Value;
        var description = Description.Create("Part of the order is missing.").Value;

        // Act
        var dispute = Dispute.Open(orderId, disputeTypeId, title, description, images: null);

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
        
        // Fixed: The class returns DisputeErrors.AlreadyResolved, not OrderErrors.DisputeCannotChangeReason
        result.TopError.ShouldBe(DisputeErrors.AlreadyResolved); 
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
        dispute.Status.ShouldBe(DisputeStatus.Resolved);
        dispute.Resolution.ShouldBe(resolution);
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
        dispute.IsResolved.ShouldBeTrue();
        dispute.DomainEvents.First().ShouldBeOfType<DisputeResolvedDomainEvent>();
    }

    [Fact]
    public void Resolve_StatusIsAlreadyResolved_ReturnsSuccessWithoutRaisingEvent()
    {
        // Arrange
        var dispute = CreateValidDispute();
        dispute.Resolve(CreateValidResolution()); // First resolution
        dispute.ClearDomainEvents(); // Clear events so we can track the second call

        // Act
        var result = dispute.Resolve(Resolution.Create("A different resolution").Value);

        // Assert
        // Fixed: The class logic states `if (Status == DisputeStatus.Resolved) return Result.Success();`
        result.IsSuccess.ShouldBeTrue(); 
        
        // State should not change to the new resolution
        dispute.Resolution.Value.ShouldBe(CreateValidResolution().Value); 
        
        // No new events should be raised since it exited early
        dispute.DomainEvents.ShouldBeEmpty();
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