using System.Collections.Generic;

namespace ModelClass.Model
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string BlogName { get; set; }
        public virtual List<Post>Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string PostTitle{ get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }
}