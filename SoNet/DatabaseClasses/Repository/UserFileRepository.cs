using DatabaseClasses.DBContext;
using DatabaseClasses.Repository.Interface;
using Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace DatabaseClasses.Repository
{
    public class UserFileRepository : IRepository<UserFile>
    {

        #region Fields
        DatabaseContext context;
        #endregion

        #region Ctor
        public UserFileRepository()
        {
            this.context = new DatabaseContext();
        }

        public UserFileRepository(DatabaseContext context)
        {
            this.context = context;
        }

        #endregion

        #region Methods
        public UserFile GetItem(object[] keys)
        {
            return context.UserFiles.Find((int)keys[0]);
        }

        public IEnumerable<UserFile> GetItems()
        {
            return context.UserFiles.AsNoTracking().ToList();
        }

        public void AddItem(UserFile item)
        {
            if (item != null)
                context.UserFiles.Add(item);
        }

        public void DeleteItem(object[] keys)
        {
            UserFile userFile = context.UserFiles.Find((int)keys[0]);
            if (userFile != null)
                context.UserFiles.Remove(userFile);
        }

        public void UpdateItem(UserFile item)
        {
            context.Entry(item).State = EntityState.Modified;
        }

        #endregion
    }
}
