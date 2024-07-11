namespace Blog.ViewModels
{
    public class CreateCommentViewModel
    {
        public string Author { get; set; }
        public string Content { get; set; }
        public int BlogPostId { get; set; }
    }
}
