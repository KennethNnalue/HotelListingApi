using HotelListingAPI.Entities;
using HotelListingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Data.Repositories
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context ;
        public CountriesRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Country> GetDetails(int id)
        {
            // Go to the countries Db context and include the hotels
            return await _context.Countries.Include( x => x.Hotels)
            .FirstOrDefaultAsync( x => x.Id == id);
            
        }
    }
}