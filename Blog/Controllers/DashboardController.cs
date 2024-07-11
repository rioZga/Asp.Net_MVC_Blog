using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        public DashboardController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Admin()
        {
            var authors = await _userManager.GetUsersInRoleAsync(UserRoles.Author);
            var authorIds = authors.Select(a => a.Id);
            authors = await _context.Users.Where(u => authorIds.Contains(u.Id)).Include(u => u.Posts).ToListAsync();
            return View(authors);
        }

        [Authorize(Roles = UserRoles.Author)]
        public async Task<IActionResult> Author()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var userPosts = _context.BlogPosts.Where(p => p.AuthorId == user.Id).ToList();
            return View(userPosts);
        }
    }
}
