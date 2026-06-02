using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;

namespace MazadZone.Application.Features.Sellers.Queries.GetFeedbacks;
public record GetSellerFeedbacksQuery(UserId SellerId, int Page, int PageSize) : IQuery<PagedList<FeedbackDto>>;