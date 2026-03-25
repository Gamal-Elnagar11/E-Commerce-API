using AutoMapper;
using E_Commerce_API.DTO.Identity;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Implementation;
using E_Commerce_API.Static;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace E_Commerce_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
     public class AccountController : ControllerBase
    {

        private readonly  IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JWTService _jwtService;
        private readonly IdentityUsers _users;
 
        public AccountController(IdentityUsers users, JWTService jwtService,IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _users = users;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var findEmail = await _userManager.FindByEmailAsync(register.Email);
            if (findEmail != null)
                return BadRequest("your email already exist");
            

            var newUser = _mapper.Map<User>(register);
            
            var result = await _userManager.CreateAsync(newUser, register.Password);
            
                

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (string.IsNullOrEmpty(newUser.UserName))
            {
                newUser.UserName = register.UserName;
            }
               
                await _userManager.AddToRoleAsync(newUser, "User");
                var token = await _jwtService.GenerateTokenAsync(newUser);

            return Ok(token);

        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
           if(!ModelState.IsValid)
                return BadRequest(ModelState);

           var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
                return Unauthorized("Invalid Email or password");

            var validPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            if(!validPassword)
                return Unauthorized("Invalid Email or password");

            var token = await _jwtService.GenerateTokenAsync(user);

            return Ok(token);

        }


        [HttpPost("AddRole")]
        public async Task<IActionResult> PromoteToAdmin(string userEmail)
        {
            // دور الـ UserManager
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound("User not found");

            if (user.Email == User.Identity.Name)
                return BadRequest("You cannot promote your selfe");

            // شيله من اليوزر لو موجود
            if (await _userManager.IsInRoleAsync(user, "User"))
                await _userManager.RemoveFromRoleAsync(user, "User");

            // ضيفه للادمن
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            return Ok($"User {userEmail} is now an Admin");
        }

        [HttpGet("AllUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _users.GetAllUsers();
            var map  = _mapper.Map<List<UsersDTO>>(users);
            return Ok(map);
        }



        [HttpPut("DeletUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var result = await _users.DeleteUser(email);

            return result switch
            {
                "Email Not Found" => NotFound("Email Not Found"),
                "Email Already Deleted" => BadRequest("Email For User is Already Deleted"),
                "Deleted Successfuly" => Ok("User Deleted Successfuly")
            };
        }







    }
}
