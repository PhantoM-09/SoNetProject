using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Post
    {
        [Required]
        public int Id { get; set; }

        public string UserEmail { get; set; }       //Создатель поста

        public string Text { get; set; }

        public string DatePublication { get; set; }

        public virtual UserData UserData { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
