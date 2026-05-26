namespace MazadZone.Application.Features.Disputes.Queries.GetFiltered;

using System;
using System.Collections.Generic;

public record GetFilteredDisputesQuery(
    string? SearchTerm,
    string? Status,
    Guid? CategoryId,
    DateTime? FromDate,
    DateTime? ToDate,
    string? SortColumn,
    bool IsDescending
) : IQuery<IReadOnlyList<DisputeListItemDto>>;