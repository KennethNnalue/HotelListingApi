using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Models.User
{
    public class UserAuthResponseDto
    {
        public string UserId{ get; set; }     
        public string Token{ get; set; }    
        public string RefreshToken {get; set;} 
    }
}