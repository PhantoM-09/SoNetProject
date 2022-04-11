using System.ComponentModel.DataAnnotations;
namespace Models
{
    public class UserFile
    {
        [Required]
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string FileName { get; set; }
        public string FileID { get; set; }

        public virtual UserData UserData { get; set; }
    }
}
