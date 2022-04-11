using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Comment
    {
        [Required]
        public int Id { get; set; }
        public int PostId { get; set; }             //К какому посту написан
        public string UserEmail { get; set; }       //Кто написал
        public string SendDate { get; set; }        //Дата и время написания
        public string Text { get; set; }            //Текст комментария

        public virtual Post Post { get; set; }
        public virtual UserData User { get; set; }
    }
}
