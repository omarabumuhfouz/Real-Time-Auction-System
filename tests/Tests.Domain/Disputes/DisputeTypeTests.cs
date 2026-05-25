using MazadZone.Domain.Orders; // Adjust namespace if actually MazadZone.Domain.Support
using MazadZone.Domain.Primitives;
using MazadZone.Domain.Shared.ValueObjects;
using Shouldly;

namespace Tests.Domain.Orders; // Adjust namespace as needed

public class DisputeTypeTests
{
    #region Factory: Create

    [Fact]
    public void Create_ValidInputs_ReturnsDisputeTypeWithDefaultState()
    {
        // Arrange
        var nameText = "Item Not Received";
        var descriptionText = "Buyer claims they never received the physical item.";

        // Act
        var result = DisputeType.Create(nameText, descriptionText);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var disputeType = result.Value;
        disputeType.Name.Value.ShouldBe(nameText);
        disputeType.Description.Value.ShouldBe(descriptionText);
        
        // Check default states
        disputeType.IsActive.ShouldBeTrue();
        disputeType.IsDeleted.ShouldBeFalse();
        disputeType.DeletedOnUtc.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_InvalidName_ReturnsFailure(string? invalidName)
    {
        // Arrange
        var validDescription = "A valid description here.";

        // Act
        var result = DisputeType.Create(invalidName!, validDescription);

        // Assert
        result.IsFailure.ShouldBeTrue();
        // Assuming Name.Create fails on empty/null strings and returns a specific error
        result.TopError.ShouldNotBeNull(); 
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_InvalidDescription_ReturnsFailure(string? invalidDescription)
    {
        // Arrange
        var validName = "Valid Name";

        // Act
        var result = DisputeType.Create(validName, invalidDescription!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        // Assuming Description.Create fails on empty/null strings and returns a specific error
        result.TopError.ShouldNotBeNull(); 
    }

    #endregion

    #region State Mutations: Update

    [Fact]
    public void Update_WhenCalled_UpdatesNameAndDescription()
    {
        // Arrange
        var disputeType = CreateValidDisputeType();
        var newName = Name.Create("Damaged Item").Value;
        var newDescription = Description.Create("Item arrived broken or damaged.").Value;

        // Act
        disputeType.Update(newName, newDescription);

        // Assert
        disputeType.Name.ShouldBe(newName);
        disputeType.Description.ShouldBe(newDescription);
    }

    #endregion

    #region State Mutations: Delete & Restore (Soft Deletable)

    [Fact]
    public void Delete_WhenNotDeleted_SetsIsDeletedFlagAndDate()
    {
        // Arrange
        var disputeType = CreateValidDisputeType();

        // Act
        var result = disputeType.Delete();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        disputeType.IsDeleted.ShouldBeTrue();
        disputeType.DeletedOnUtc.ShouldNotBeNull();
        disputeType.DeletedOnUtc.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ReturnsSuccessAndLeavesDateUnchanged()
    {
        // Arrange
        var disputeType = CreateValidDisputeType();
        disputeType.Delete(); // First delete
        var originalDeletedDate = disputeType.DeletedOnUtc;

        // Simulate a slight delay to ensure time would change if re-assigned
        Thread.Sleep(10); 

        // Act
        var result = disputeType.Delete(); // Second delete

        // Assert
        result.IsSuccess.ShouldBeTrue();
        disputeType.IsDeleted.ShouldBeTrue();
        disputeType.DeletedOnUtc.ShouldBe(originalDeletedDate); // Date should not overwrite
    }

    [Fact]
    public void Restore_WhenDeleted_ResetsIsDeletedFlagAndDate()
    {
        // Arrange
        var disputeType = CreateValidDisputeType();
        disputeType.Delete(); // Delete it first

        // Act
        var result = disputeType.Restore();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        disputeType.IsDeleted.ShouldBeFalse();
        disputeType.DeletedOnUtc.ShouldBeNull();
    }

    #endregion

    // --- Core Factory Helpers ---

    private static DisputeType CreateValidDisputeType()
    {
        return DisputeType.Create("Default Name", "Default Description").Value;
    }
}