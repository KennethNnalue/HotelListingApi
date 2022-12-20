using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListingAPI.Interfaces;
using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UsersAccountController : ControllerBase
    {
        private readonly IUserAuthManagerRepository _userAuthManager;
         private readonly IMapper _mapper;
        private readonly ILogger<UsersAccountController> _logger;

        public UsersAccountController(
            IUserAuthManagerRepository userAuthManager, 
            IMapper mapper,
            ILogger<UsersAccountController> logger
            )
        {
            _userAuthManager = userAuthManager;
            _mapper = mapper;
            _logger = logger;
        }

             
        // // GET: api/UsersAccount/users
        // [HttpGet]
        // [Route("users")]
        // public async Task<ActionResult<IEnumerable<GetUserDto>>> GetUsers()
        // {
        //    var users = await _userAuthManager.GetAllAsync();

        //   var  results = _mapper.Map<List<GetUserDto>>(users);

        //   return Ok(results);
           
        // }


           // GET: api/UsersAccount/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] CreateUserDto user)
        {

            var errors = await _userAuthManager.Register(user);

            if(errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();
           
            // catch (System.Exception exception )
            // {
            //     _logger.LogError(exception , $"Something Went Wrong in the {nameof(Register)} - User Registration attempt for {user.Email} ");
            //     return Problem($"Something Went Wrong in the {nameof(Register)}.  Please contact support. ", statusCode: 500);
            // }          
            
        }


         // GET: api/UsersAccount/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {

             var userAuthResponse = await _userAuthManager.Login(loginUserDto);
            if(userAuthResponse == null)
            {
                return Unauthorized();
            }

            return Ok(userAuthResponse);
        

        }

         // GET: api/UsersAccount/refreshToken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] UserAuthResponseDto request)
        {
            var userAuthResponse = await _userAuthManager.VerifyRefreshToken(request);
            if(userAuthResponse == null)
            {
                return Unauthorized();
            }

            return Ok(userAuthResponse);

        }
    

         // PUT: api/UsersAccount/refreshToken
       
        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Administrator")]
         public async Task<IActionResult> AddAdminRole(string id)
         {
            var userAuthResponse = await _userAuthManager.AddAdminRoles(id);
            if(userAuthResponse == null)
            {
                return NotFound();
            }

            return Ok();
         }
    }
}