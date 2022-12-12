using AutoMapper;
using HotelListingAPI.Entities;
using HotelListingAPI.Models.Country;
using HotelListingAPI.Models.Hotel;
using HotelListingAPI.Models.User;

namespace HotelListingAPI.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
             CreateMap<Country, CreateCountryDto>().ReverseMap();
             CreateMap<Country, GetCountryDto>().ReverseMap();
             CreateMap<Country, GetCountryDetailsDto>().ReverseMap();
             CreateMap<Country, UpdateCountryDto>().ReverseMap();
              
             CreateMap<Hotel, GetHotelDto>().ReverseMap();
             CreateMap<Hotel, CreateHotelDto>().ReverseMap();
             CreateMap<Hotel, UpdateHotelDto>().ReverseMap();

             CreateMap<User, CreateUserDto>().ReverseMap();
             CreateMap<User, GetUserDto>().ReverseMap();
        }
    }
}