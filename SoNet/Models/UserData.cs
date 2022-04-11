using System.Collections.Generic;


namespace Models
{
    public class UserData
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        public string BirthDay { get; set; }

        public string Country { get; set; }
        public string ImageName { get; set; }
        public string ImageId { get; set; }
        public bool IsOnline { get; set; } = false;
        public bool IsBlocked { get; set; }

        public virtual ICollection<Friend> Users { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserFile> Files { get; set; }
    }
}
