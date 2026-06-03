namespace MazadZone.Domain.Auctions;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MazadZone.Domain.Auctions.Enums;
using MazadZone.Domain.Auctions.ValueObjects;
using MazadZone.Domain.Categories;
using MazadZone.Domain.Items;
using MazadZone.Domain.Primitives;
using MazadZone.Domain.Shared.ValueObjects;

public sealed class Item : Entity<ItemId>
{
    private readonly List<Image> _images = new();

    private Item() { }

    private Item(
        ItemId id,
        AuctionId auctionId,
        CategoryId categoryId,
        ItemStatus status,
        Description condition,
        string title,
        List<Image> images,
        Description description) : base(id)
    {
        AuctionId = auctionId;
        CategoryId = categoryId;
        Status = status;
        Condition = condition;
        Title = title;
        _images = images;
        Description = description;
    }

    // --- Properties ---
    public CategoryId CategoryId { get; private set; }
    public AuctionId AuctionId { get; private set; }
    public ItemStatus Status { get; private set; }
    public Description Condition { get; private set; }
    public string Title { get; private set; }
    public Description Description { get; private set; }

    // Encapsulated collection
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    // --- Factory Method ---
    public static Result<Item> Create(
        AuctionId auctionId,
        CategoryId categoryId,
        ItemStatus status,
        Description condition,
        string title,
        string description,
        List<Image> images
    )
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > AuctionConstants.MaxTitleLength)
            return ItemErrors.InvalidTitle;

        if (condition is null)
            return MazadZone.Domain.Shared.Errors.DescriptionErrors.Empty;

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return ItemErrors.InvalidDescription;

        foreach (var image in images)
        {
            var imageResult = Image.Create(image.Path, image.AltText, image.IsMain);

            if (imageResult.IsFailure)
                return imageResult.TopError;
        }

        return new Item(
            ItemId.New(),
            auctionId,
            categoryId,
            status,
            condition,
            title,
            images,
            descriptionResult.Value
        );
    }
}