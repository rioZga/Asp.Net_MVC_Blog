﻿namespace Blog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual List<Comment>? Comments { get; set; } = null;
    }
}
