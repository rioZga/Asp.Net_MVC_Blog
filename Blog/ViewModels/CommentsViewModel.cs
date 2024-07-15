using Blog.Models;

namespace Blog.ViewModels
{
    public class CommentsViewModel
    {
        public int BlogPostId { get; set; }
        public List<Comment> Comments { get; set; }
        public NewComment NewComment { get; set; }

    }

    public class NewComment
    {
        public int BlogPostId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}
