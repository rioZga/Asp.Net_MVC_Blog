using Blog.Data;
using Blog.Models;
using BlogWebApi.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Name);
            if (user == null)
            {
                return BadRequest();
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCheck)
            {
                return BadRequest();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
