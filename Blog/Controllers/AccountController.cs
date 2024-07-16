using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByNameAsync(loginVM.Name);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials, please try again.");
                return View(loginVM);
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if (!passwordCheck)
            {
                ModelState.AddModelError("", "Invalid credentials, please try again.");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid credentials, please try again.");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(registerVM);
            }

            var user = await _userManager.FindByNameAsync(registerVM.Name);
            if (user != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(registerVM);
            }

            var newUser = new User
            {
                Name = registerVM.Name,
                UserName = registerVM.Name,
            };

            
            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "An error occurred, please try again!");
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.Author);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User does not exist");
                return View("Admin");
            }

            var blogPosts = _context.BlogPosts.Where(p => p.Author.Id == user.Id);
            _context.BlogPosts.RemoveRange(blogPosts);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View("Admin");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Admin", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
