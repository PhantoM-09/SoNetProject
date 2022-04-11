
namespace Models
{
    public class Friend
    {
        public string UserEmail { get; set; }

        public string FriendEmail { get; set; }

        public int Status { get; set; }

        public virtual UserData UserInfo { get; set; }
        public virtual UserData FriendInfo { get; set; }
    }
}
