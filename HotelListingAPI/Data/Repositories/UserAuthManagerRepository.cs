using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListingAPI.Entities;
using HotelListingAPI.Interfaces;
using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.Data.Repositories
{
    public class UserAuthManagerRepository :  GenericRepository<User>, IUserAuthManagerRepository
    {
        //UserManager 
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly HotelListingDbContext _context;
        private readonly IConfiguration _configuration;
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        private User _user;
        public UserAuthManagerRepository(
        HotelListingDbContext context, 
        IMapper mapper, 
        UserManager<User>  userManager,
        IConfiguration configuration
        ) : base(context)
        {
            this._context = context;
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider , _refreshToken);

            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);

            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);

            return  newRefreshToken;
        }   

          public  async Task<UserAuthResponseDto> VerifyRefreshToken(UserAuthResponseDto request)
        {
            var JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = JwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type ==
            JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if(_user == null || _user.Id != request.UserId)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider,
             _refreshToken, request.RefreshToken);

            if(isValidRefreshToken)
            {
                var token = await GenerateToken();

                return new UserAuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }

        public async Task<UserAuthResponseDto> Login(LoginUserDto loginUserDto)
        {   
            _user = await _userManager.FindByEmailAsync(loginUserDto.Email);

            bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginUserDto.Password);

            if(_user == null || isValidUser == false)
            {
                return null;
            }

            var token = await GenerateToken();
            return new UserAuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };

        }

        public async Task<IEnumerable<IdentityError>> Register(CreateUserDto userDto)
        {
            _user = _mapper.Map<User>(userDto);

            //use user's email as the username
            _user.UserName = userDto.Email;
            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
                //await _userManager.AddToRoleAsync(_user, "Administrator");
            }

            return result.Errors;

            
        }

         public async Task<IEnumerable<IdentityError>> AddAdminRoles(string email)
        {
           
            _user = await _userManager.FindByEmailAsync(email);

            if(_user == null)
            {
                return null;
            }

          var result =  await _userManager.AddToRoleAsync(_user, "Administrator");
          

            return result.Errors;

            
        }

      

        private async Task<string> GenerateToken()
         {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id),
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
            
         }

       
    }
}