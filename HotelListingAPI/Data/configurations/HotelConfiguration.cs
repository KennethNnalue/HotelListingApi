using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.Data.configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
				new Hotel
				{
					Id = 1,
					Name = "Tboy Resort",
					Address = "Awka, Nigeria",
					CountryId = 1,
					Rating =4.8
				},
				new Hotel
				{
					Id = 2,
					Name = "Emily Suites",
					Address = "Accra, Ghana",
					CountryId = 2,
					Rating =4.2
				},
				new Hotel
				{
					Id = 3,
					Name = "Hammer Hotel",
					Address = "Hamm, Germany",
					CountryId = 3,
					Rating =4.4
				}
			);
        }
    }
}


