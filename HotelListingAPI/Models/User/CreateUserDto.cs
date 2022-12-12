using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace HotelListingAPI.Models.User
{
    public class CreateUserDto : BaseUserDto
    {   [Required]
        public string FirstName{ get; set; }

        [Required]
        public string LastName{ get; set; }

    }
}