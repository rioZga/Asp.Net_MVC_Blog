using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Blog.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpClientFactory _factory;

        public BlogPostController(AppDbContext context, UserManager<User> userManager, IHttpClientFactory factory)
        {
            _context = context;
            _userManager = userManager;
            _factory = factory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = _factory.CreateClient("blogWebApi");
            var blog = await client.GetFromJsonAsync<BlogPost>($"api/BlogPost/{id}");

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
        public async Task<IActionResult> Create(CreateBlogPostViewModel blogPostVM)
        {
            if (!ModelState.IsValid)
            {
                return View(blogPostVM);
            }

            string? imageUrl = null;

            if (blogPostVM.Image != null)
            {
                var client = _factory.CreateClient("blogWebApi");
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(blogPostVM.Image.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                content.Add(fileContent, "file", blogPostVM.Image.FileName);

                var response = await client.PostAsync("/api/image/upload", content);
                if (response.IsSuccessStatusCode)
                    imageUrl = await response.Content.ReadAsStringAsync();
            }

            var blogPost = new BlogPost
            {
                Title = blogPostVM.Title,
                AuthorId = blogPostVM.AuthorId,
                Body = blogPostVM.Body,
                CreatedAt = DateTime.Now,
                Tags = blogPostVM.Tags,
                Image = imageUrl,
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

            if (!String.IsNullOrEmpty(blog.Image))
            {
                var client = _factory.CreateClient("blogWebApi");
                client.PostAsJsonAsync("/api/image/delete", new
                {
                    Url = blog.Image
                });
            }

            _context.BlogPosts.Remove(blog);
            _context.SaveChanges();
            return Redirect("/");
        }
    }
}
