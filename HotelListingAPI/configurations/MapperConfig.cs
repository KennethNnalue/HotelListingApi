using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListingAPI.Entities;
using HotelListingAPI.Models.Country;

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
        }
    }
}