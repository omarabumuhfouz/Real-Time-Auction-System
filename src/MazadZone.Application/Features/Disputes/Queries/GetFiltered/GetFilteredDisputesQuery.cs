namespace MazadZone.Application.Features.Disputes.Queries.GetFiltered;

using System;
using MazadZone.Application.Common.Paging;

public record GetFilteredDisputesQuery(
    string? SearchTerm,
    string? Status,
    Guid? CategoryId,
    DateTime? FromDate,
    DateTime? ToDate,
    string? SortColumn,
    bool IsDescending,
    int PageNumber,
    int PageSize
) : IQuery<PagedList<DisputeListItemDto>>;