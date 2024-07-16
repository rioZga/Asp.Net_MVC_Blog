using Blog.Data;
using Blog.Models;
using BlogWebApi.Dtos;
using BlogWebApi.Helpers;
using BlogWebApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BlogWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHelper _jwtHelper;

        public AuthService(UserManager<User> userManager, JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Name);
            if (user == null)
            {
                return new Response { Message = "Invalid Credentials!" };
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCheck)
            {
                return new Response { Message = "Invalid Credentials!" };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var jwtSecurityToken = _jwtHelper.CreateJwtToken(user, userClaims, userRoles);

            return new Response
            {
                Success = true,
                Username = user.UserName,
                Roles = new List<string> { UserRoles.Author },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
            };
        }

        public async Task<Response> Register(RegisterDto registerDto)
        {
            var user = await _userManager.FindByNameAsync(registerDto.Name);
            if (user != null)
                return new Response { Message = "Username already exists!" };

            var newUser = new User
            {
                Name = registerDto.Name,
                UserName = registerDto.Name,
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description}, ");

                return new Response { Message = errors.ToString() };
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.Author);

            var userRoles = await _userManager.GetRolesAsync(newUser);
            var userClaims = await _userManager.GetClaimsAsync(newUser);

            var jwtSecurityToken = _jwtHelper.CreateJwtToken(newUser, userClaims, userRoles);


            return new Response
            {
                Success = true,
                Username = newUser.UserName,
                Roles = new List<string> { UserRoles.Author },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
            };
        }
    }
}
