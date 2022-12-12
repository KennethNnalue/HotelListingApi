using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingAPI.Entities;
using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.Interfaces
{
    public interface IUserAuthManagerRepository 
    {

      
        Task<IEnumerable<IdentityError>> Register(CreateUserDto userDto);
        Task<UserAuthResponseDto> Login(LoginUserDto loginUserDto);

        Task<string> CreateRefreshToken();

         Task<UserAuthResponseDto> VerifyRefreshToken(UserAuthResponseDto request );

         Task<IEnumerable<IdentityError>> AddAdminRoles(string email);

    }
}