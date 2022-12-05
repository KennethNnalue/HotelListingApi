using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingAPI.Entities;

namespace HotelListingAPI.Interfaces
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        Task<Country> GetDetails(int id);
    }
}