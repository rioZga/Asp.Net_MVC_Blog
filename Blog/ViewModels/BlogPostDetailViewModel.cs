using Blog.Models;

namespace Blog.ViewModels
{
    public class BlogPostDetailViewModel
    {
        public BlogPost BlogPost { get; set; }
        public Comment NewComment { get; set; }
    }
}
