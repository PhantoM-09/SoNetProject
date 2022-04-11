using Models;
using System.Data.Entity;


namespace DatabaseClasses.DBContext
{
    public class DatabaseContext : DbContext
    {
        #region Ctor
        public DatabaseContext() : base("DBConnect")
        {

        }
        #endregion

        #region Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Первичные ключи-----------------------------------------------------------------------------------------------------

            //Установка первичного ключа в виде поля Email в таблице UserDatas 
            modelBuilder.Entity<UserData>().HasKey(u => u.Email);

            //Установка составного первичного ключа в виде полей UserEmail и FriendEmail в таблице Friends
            modelBuilder.Entity<Friend>().HasKey(u => new { u.UserEmail, u.FriendEmail });

            //Установка первичного ключа в виде поля Id в таблице Posts 
            modelBuilder.Entity<Post>().HasKey(u => u.Id);

            //Установка первичного ключа в виде поля Id в таблице Comments
            modelBuilder.Entity<Comment>().HasKey(u => u.Id);

            //Установка первичного ключа в виде поля Id в таблице Files
            modelBuilder.Entity<UserFile>().HasKey(u => u.Id);
            //Внешние ключи----------------------------------------------------------------------------------------------------------------------------------------------------

            //Таблицы UserDatas и Friends-------------------------------------------------------------------------------------------------------------------------
            //Установка внешнего ключа UserEmail в таблице Friend. Связь один ко многим. Каскадное удаление разрешено
            modelBuilder.Entity<UserData>().HasMany(u => u.Users).WithRequired(p => p.UserInfo).HasForeignKey(f => f.UserEmail).WillCascadeOnDelete(true);
            //Установка внешнего ключа FriendEmail в таблице Friend. Связь один ко многим. Каскадное удаление запрещено
            modelBuilder.Entity<UserData>().HasMany(u => u.Friends).WithRequired(p => p.FriendInfo).HasForeignKey(f => f.FriendEmail).WillCascadeOnDelete(false);
            //----------------------------------------------------------------------------------------------------------------------------------------------------


            //Таблицы UserDatas и Posts---------------------------------------------------------------------------------------------------------------------------
            //Установка внешнего ключа UserEmail в таблице Post.Связь один ко многим. Каскадное удаление разрешено
            modelBuilder.Entity<UserData>().HasMany(u => u.Posts).WithRequired(p => p.UserData).HasForeignKey(f => f.UserEmail).WillCascadeOnDelete(true);
            //----------------------------------------------------------------------------------------------------------------------------------------------------


            //Таблицы Posts и Comments----------------------------------------------------------------------------------------------------------------------------
            //Установка внешнего ключа PostId в таблице Comment. Связь один ко многим. Каскадное удаление разрешено
            modelBuilder.Entity<Post>().HasMany(u => u.Comments).WithRequired(p => p.Post).HasForeignKey(f => f.PostId).WillCascadeOnDelete(true);
            //----------------------------------------------------------------------------------------------------------------------------------------------------


            //Таблицы UserDatas и Comments------------------------------------------------------------------------------------------------------------------------
            //Установка внешнего ключа PostId в таблице Comment. Связь один ко многим. Каскадное удаление разрешено
            modelBuilder.Entity<UserData>().HasMany(u => u.Comments).WithRequired(p => p.User).HasForeignKey(f => f.UserEmail).WillCascadeOnDelete(false);
            //----------------------------------------------------------------------------------------------------------------------------------------------------


            //Таблицы UserDatas и Files---------------------------------------------------------------------------------------------------------------------------
            //Установка внешнего ключа UserEmail в таблице Post.Связь один ко многим. Каскадное удаление разрешено
            modelBuilder.Entity<UserData>().HasMany(u => u.Files).WithRequired(p => p.UserData).HasForeignKey(f => f.UserEmail).WillCascadeOnDelete(true);
            //----------------------------------------------------------------------------------------------------------------------------------------------------

        }

        public DbSet<UserData> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
        #endregion
    }
}
