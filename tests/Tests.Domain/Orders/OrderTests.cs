using MazadZone.Domain.Auctions;
using MazadZone.Domain.Orders;
using MazadZone.Domain.Orders.Events;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users.ValueObjects;
using Shouldly;

namespace Tests.Domain.Orders;

public class OrderTests
{
    private static Order CreateValidPendingOrder()
    {
        var address = new Address("123 Test St", "Amman", "11118", "Jordan");
        return Order.Create(
            AuctionId.New(),
            UserId.New(),
            BidId.New(),
            address,
            150.00m).Value;
    }


    [Fact]
    public void Create_TotalAmountIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var address = new Address("123 Test St", "Amman", "11118", "Jordan");

        // Act
        var result = Order.Create(AuctionId.New(), UserId.New(), BidId.New(), address, -10m);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.TotalAmountTooLow); // Or whatever error Money.Create returns
    }

    [Fact]
    public void Create_ValidParameters_InitializesOrder()
    {
        // Act
        var order = CreateValidPendingOrder();

        // Assert
        order.Status.ShouldBe(OrderStatus.Pending);
        order.TotalAmount.Amount.ShouldBe(150.00m);
        
        // Verify Domain Event was raised (Assuming your AggregateRoot exposes DomainEvents)
        var domainEvent = order.DomainEvents.OfType<OrderCreatedDomainEvent>().SingleOrDefault();
        domainEvent.ShouldNotBeNull();
        domainEvent.OrderId.ShouldBe(order.Id);
    }

    // --- 2. STATE MACHINE (Confirm, Ship, Deliver, Cancel) ---

    [Fact]
    public void Cancel_OrderIsNotPending_ReturnsCannotCancelError()
    {
        // Arrange
        var order = CreateValidPendingOrder();
        order.Confirm(); // Move to Confirmed

        // Act
        var result = order.Cancel();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.CannotCancel);
    }

    [Fact]
    public void Cancel_OrderIsPending_CancelsOrder()
    {
        var order = CreateValidPendingOrder();
        var result = order.Cancel();

        result.IsSuccess.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Canceled);
        order.DomainEvents.OfType<OrderCancelledDomainEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Confirm_OrderIsNotPending_ReturnsCannotConfirmError()
    {
        var order = CreateValidPendingOrder();
        order.Cancel(); // Move out of Pending

        var result = order.Confirm();

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.CannotConfirm);
    }

[Fact]
    public void Confirm_OrderIsPending_ConfirmsOrder()
    {
        // Arrange
        var order = CreateValidPendingOrder(); // Status is Pending

        // Act
        var result = order.Confirm();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Confirmed);
        order.DomainEvents.OfType<OrderConfirmedDomainEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Ship_OrderIsNotConfirmed_ReturnsCannotShippedError()
    {
        var order = CreateValidPendingOrder(); // Status is Pending

        var result = order.Ship();

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.CannotShipped);
    }

[Fact]
    public void Ship_OrderIsConfirmed_ShipsOrder()
    {
        // Arrange
        var order = CreateValidPendingOrder();
        order.Confirm(); // Move to Confirmed

        // Act
        var result = order.Ship();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Shipped);
        order.DomainEvents.OfType<OrderShippedDomainEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Deliver_OrderIsShipped_DeliversOrder()
    {
        // Arrange: Walk the state machine forward
        var order = CreateValidPendingOrder();
        order.Confirm();
        order.Ship();

        // Act
        var result = order.Deliver();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Delivered);
        order.DomainEvents.OfType<OrderDeliveredDomainEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Deliver_OrderIsNotShipped_ReturnsCannotDeliverError()
    {
        // Arrange
        var order = CreateValidPendingOrder();
        order.Confirm(); // Status is Confirmed, skipping the 'Shipped' state

        // Act
        var result = order.Deliver();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.CannotDeliver);
    }

    // --- 3. FEEDBACK ---

    [Fact]
    public void AddFeedback_OrderIsNotDelivered_ReturnsFeedbackRequiresDeliveredError()
    {
        var order = CreateValidPendingOrder(); // Pending state

        var result = order.AddFeedback(5, "Great");

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.FeedbackRequiresDelivered);
    }

    [Fact]
    public void AddFeedback_FeedbackAlreadyExists_ReturnsFeedbackAlreadyExistsError()
    {
        var order = CreateValidPendingOrder();
        order.Confirm(); order.Ship(); order.Deliver(); // Move to valid state
        
        order.AddFeedback(5, "First"); // Add initial feedback

        // Act: Try again
        var result = order.AddFeedback(1, "Second");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.FeedbackAlreadyExists);
    }

    [Fact]
    public void AddFeedback_ValidParameters_AddsFeedback()
    {
        var order = CreateValidPendingOrder();
        order.Confirm(); order.Ship(); order.Deliver();

        var result = order.AddFeedback(5, "Perfect");

        result.IsSuccess.ShouldBeTrue();
        order.Feedback.ShouldNotBeNull();
        order.DomainEvents.OfType<FeedbackLeftDomainEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ReplyToFeedback_NoFeedbackExists_ReturnsNoFeedbackError()
    {
        var order = CreateValidPendingOrder();

        var result = order.ReplyToFeedback("Thanks!");

        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(OrderErrors.NoFeedback);
    }

[Fact]
    public void ReplyToFeedback_FeedbackExists_RepliesToFeedback()
    {
        // Arrange: Build the entire state chain required to leave a reply
        var order = CreateValidPendingOrder();
        order.Confirm(); 
        order.Ship(); 
        order.Deliver();
        order.AddFeedback(5, "Fast shipping, great item!"); // Buyer leaves feedback

        // Act: Seller replies
        var result = order.ReplyToFeedback("Thank you for your purchase!");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // Verify the event was raised to notify the buyer
        order.DomainEvents.OfType<FeedbackRepliedDomainEvent>().ShouldHaveSingleItem();
    }

    }