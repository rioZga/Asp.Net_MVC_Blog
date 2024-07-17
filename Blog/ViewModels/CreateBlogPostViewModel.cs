using Blog.Models;

namespace Blog.ViewModels
{
    public class CreateBlogPostViewModel
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
