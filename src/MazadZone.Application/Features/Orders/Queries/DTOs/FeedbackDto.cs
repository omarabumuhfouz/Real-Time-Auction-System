namespace MazadZone.Application.Features.Orders.Queries.DTOs;

public record FeedbackDto(Guid Id,string AuthorName, int Rating, string Comment, string Reply, DateTime CreatedAt);