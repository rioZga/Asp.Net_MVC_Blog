using Microsoft.AspNetCore.Mvc;
using Blog.ViewModels;
using Blog.Models;
using Blog.Data;
using System.Reflection.Metadata;

namespace Blog.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _contex;
        public CommentController(AppDbContext context)
        {
            _contex = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Comment(NewComment newComment)
        {

            var comment = new Comment
            {
                Author = newComment.Author,
                Content = newComment.Content,
                BlogPostId = newComment.BlogPostId,
                CreatedAt = DateTime.Now,
            };

            _contex.Comments.Add(comment);
            _contex.SaveChanges();
            return Redirect($"/blogpost/detail/{comment.BlogPostId}");

        }
    
    }
}
