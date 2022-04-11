using DatabaseClasses.DBContext;
using DatabaseClasses.Repository.Interface;
using Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace DatabaseClasses.Repository
{
    public class PostRepository : IRepository<Post>
    {
        #region Fields
        DatabaseContext context;
        #endregion

        #region Ctor
        public PostRepository()
        {
            this.context = new DatabaseContext();
        }

        public PostRepository(DatabaseContext context)
        {
            this.context = context;
        }

        #endregion

        #region Methods
        public Post GetItem(object[] keys)
        {
            return context.Posts.Find((int)keys[0]);
        }

        public IEnumerable<Post> GetItems()
        {
            return context.Posts.AsNoTracking().ToList();
        }

        public void AddItem(Post item)
        {
            if (item != null)
                context.Posts.Add(item);
        }

        public void DeleteItem(object[] keys)
        {
            Post post = context.Posts.Find((int)keys[0]);
            if (post != null)
                context.Posts.Remove(post);
        }

        public void UpdateItem(Post item)
        {
            context.Entry(item).State = EntityState.Modified;
        }

        #endregion

    }
}
