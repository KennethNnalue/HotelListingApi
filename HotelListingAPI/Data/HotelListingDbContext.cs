using System;
using HotelListingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Data
{
	public class HotelListingDbContext : DbContext
	{
		public HotelListingDbContext(DbContextOptions  options) : base(options)
		{
		}

		public DbSet<Hotel> Hotels { get; set; }

		public DbSet<Country> Countries { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Country>().HasData(
				new Country
				{
					Id = 1,
					Name = "Jamaica",
					ShortName = "JM"
				},
				new Country
				{
					Id = 2,
					Name = "Nigeria",
					ShortName = "NG"
				},
				new Country
				{
					Id = 3,
					Name = "Germany",
					ShortName = "DE"
				}
			);

			modelBuilder.Entity<Hotel>().HasData(
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

