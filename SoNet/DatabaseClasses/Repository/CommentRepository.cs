
using DatabaseClasses.DBContext;
using DatabaseClasses.Repository.Interface;
using Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DatabaseClasses.Repository
{
    public class CommentRepository : IRepository<Comment>
    {
        #region Fields
        DatabaseContext context;
        #endregion

        #region Ctor
        public CommentRepository()
        {
            this.context = new DatabaseContext();
        }

        public CommentRepository(DatabaseContext context)
        {
            this.context = context;
        }

        #endregion

        #region Methods
        public Comment GetItem(object[] keys)
        {
            return context.Comments.Find((int)keys[0]);
        }

        public IEnumerable<Comment> GetItems()
        {
            return context.Comments.AsNoTracking().Include(p=>p.User).ToList();
        }

        public void AddItem(Comment item)
        {
            if (item != null)
                context.Comments.Add(item);
        }

        public void DeleteItem(object[] keys)
        {
            Comment comment = context.Comments.Find((int)keys[0]);
            if (comment != null)
                context.Comments.Remove(comment);
        }

        public void UpdateItem(Comment item)
        {
            context.Entry(item).State = EntityState.Modified;
        }

        #endregion
    }
}
