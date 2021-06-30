using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnoAPI.Data;
using UnoAPI.Data.Models;
using UnoAPI.Helpers;
using UnoAPI.Inteface;

namespace UnoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly UnoContext _context;

        public UsersController(IUserHelper userHelper, IConfiguration configuration, UnoContext context)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> CreateToken(Credentials model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user != null)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Created(string.Empty, GenerateToken(user));
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(RegisterData data)
        {
            await _userHelper.AddUserAsync(data.User, data.Password);
            await _userHelper.AddUserToRoleAsync(data.User, Roles.Customer.ToString());
            return Created(string.Empty, GenerateToken(data.User));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutUser(UserData user)
        {
            string result;
            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                User currentUser = await _context.Users.SingleAsync(u => u.Email == email);
                currentUser.Email = user.Email;
                currentUser.UserName = user.UserName;
                currentUser.FirstNames = user.FirstNames;
                currentUser.SecondNames = user.SecondNames;
                currentUser.PhoneNumber = user.PhoneNumber;
                _context.Entry(currentUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                result = JsonConvert.SerializeObject(user);
            }
            catch(Exception ex)
            {
                result = JsonConvert.SerializeObject(ex);
            }
            return Content(result);
        }

        /*[HttpPost]
        [Route("Roles")]
        public async Task<List<string>> PostRoles()
        {
            await _userHelper.CheckRoleAsync(Roles.Admin.ToString());
            await _userHelper.CheckRoleAsync(Roles.Customer.ToString());
            await _userHelper.CheckRoleAsync(Roles.Worker.ToString());
            return new List<string> { Roles.Admin.ToString(), Roles.Customer.ToString(), Roles.Worker.ToString() };
        }*/

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _userHelper.LogoutAsync();
            return Ok();
        }

        private object GenerateToken(User user)
        {
            Claim[] claims = new[]
                    {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(3),
                signingCredentials: credentials);
            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user
            };
        }
    }

    public class RegisterData
    {
        public string Password { get; set; }

        public User User { get; set; }
    }
}
