namespace MazadZone.Api.Contracts.Bidders;

using MazadZone.Application.Features.Bidders.DTOs;

public record RegisterBidderRequest(
    string Email,
    string Password,
    string PhoneNumber,
    string NationalId,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    AddressDto Address
);