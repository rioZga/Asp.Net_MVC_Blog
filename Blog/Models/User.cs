using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public virtual List<BlogPost>? Posts { get; set; }
        
    }
}
