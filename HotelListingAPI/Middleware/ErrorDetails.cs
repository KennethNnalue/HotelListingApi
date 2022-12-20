using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Middleware
{
    public class ErrorDetails
    {
        public string ErrorType { get; set; }   
        public string ErrorMessage { get; set; }
    }
}