using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Models.Country
{
    public class GetCountryDetailsDto: BaseCountryDto
    {
            public int Id { get; set; }
            public List<GetHotelDto> Hotels { get; set; }
        
    }
}