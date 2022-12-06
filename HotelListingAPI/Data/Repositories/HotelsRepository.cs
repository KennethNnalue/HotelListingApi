using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingAPI.Entities;
using HotelListingAPI.Interfaces;

namespace HotelListingAPI.Data.Repositories
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListingDbContext _context;
        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }
    }
}