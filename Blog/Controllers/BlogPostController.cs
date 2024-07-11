using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public BlogPostController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var blog = _context.BlogPosts.Include(p => p.Comments).Include(p => p.Author).FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            var blogPostDetail = new BlogPostDetailViewModel
            {
                BlogPost = blog,
            };
            return View(blogPostDetail);

        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Author)]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            var createBlogPostViewModel = new CreateBlogPostViewModel
            {
                AuthorId = currentUser.Id,
            };
            return View(createBlogPostViewModel);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Author)]
        public IActionResult Create(CreateBlogPostViewModel blogPostVM)
        {
            if (!ModelState.IsValid)
            {
                return View(blogPostVM);
            }

            var blogPost = new BlogPost
            {
                Title = blogPostVM.Title,
                AuthorId = blogPostVM.AuthorId,
                Body = blogPostVM.Body,
                CreatedAt = DateTime.Now,
                Tags = blogPostVM.Tags,
            };
            
            _context.BlogPosts.Add(blogPost);
            _context.SaveChanges();
            return Redirect("/");
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Author)]
        public IActionResult Edit(int id)
        {
            var blog = _context.BlogPosts.FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            var blogPostVM = new EditBlogPostViewModel
            {
                Title = blog.Title,
                Body = blog.Body,
                Tags = blog.Tags,
            };

            return View(blogPostVM);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Author)]
        public IActionResult Edit(int id, EditBlogPostViewModel blogVM)
        {
            if (!ModelState.IsValid)
            {
                return View(blogVM);
            }

            var blog = _context.BlogPosts.AsNoTracking().FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = blogVM.Title;
            blog.Body = blogVM.Body;
            blog.Tags = blogVM.Tags;

            _context.BlogPosts.Update(blog);
            _context.SaveChanges();
            return Redirect($"/blogpost/detail/{blog.Id}");
        }

        [Authorize(Roles = UserRoles.Author)]
        public IActionResult Delete(int id)
        {
            var blog = _context.BlogPosts.FirstOrDefault(p => p.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            _context.BlogPosts.Remove(blog);
            _context.SaveChanges();
            return Redirect("/");
        }
    }
}
