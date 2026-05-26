namespace MazadZone.Domain.Orders; // Or MazadZone.Domain.Support

using MazadZone.Domain.Primitives;
using MazadZone.Domain.Shared.ValueObjects;

public sealed class DisputeType : AggregateRoot<DisputeTypeId> , IAuditableEntity
{
    private DisputeType() { }

    private DisputeType(DisputeTypeId id, Name name, Description description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    
    public bool IsActive { get; private set; } = true; 
    public DateTime? DeletedOnUtc { get; private set; }

    public DateTime CreatedOnUtc { get; set ; }
    public DateTime? ModifiedOnUtc { get ; set ; }

    public static Result<DisputeType> Create(string name, string description)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure) return nameResult.TopError;

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure) return descriptionResult.TopError;

        return new DisputeType(DisputeTypeId.New(), nameResult.Value, descriptionResult.Value);
    }

    public void Update(Name newName, Description newDescription)
    {
        Name = newName;
        Description = newDescription;
    }

    public Result Delete()
    {
        if (!IsActive) return Result.Success();
        IsActive = false;
        DeletedOnUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Restore()
    {
        IsActive = true;
        DeletedOnUtc = null;
        return Result.Success();
    }
}