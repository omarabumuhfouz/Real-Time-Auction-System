using AutoMapper;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Entities.Users;

public class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<AddressDto, Address>().ReverseMap();
    }
}
