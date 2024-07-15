using Blog.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BlogPostController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetPostById(int id)
        {
            var blog = _context.BlogPosts.Include(p => p.Comments).Include(p => p.Author).FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }
    }
}
