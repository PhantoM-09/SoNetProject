using DatabaseClasses.DBContext;
using DatabaseClasses.Repository.Interface;
using Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;



namespace DatabaseClasses.Repository
{
    public class UserDataRepository : IRepository<UserData>
    {
        #region Fields
        DatabaseContext context;
        #endregion

        #region Ctor
        public UserDataRepository()
        {
            this.context = new DatabaseContext();
        }

        public UserDataRepository(DatabaseContext context)
        {
            this.context = context;
        }
        #endregion


        #region Methods
        public UserData GetItem(object[] keys)
        {
            return context.Users.Find(keys[0] as string);
        }

        public IEnumerable<UserData> GetItems()
        {
            return context.Users.AsNoTracking().ToList();
        }

        public void AddItem(UserData item)
        {
            if (item != null)
                context.Users.Add(item);
        }

        public void DeleteItem(object[] keys)
        {
            UserData userData = context.Users.Find(keys[0] as string);
            if (userData != null)
                context.Users.Remove(userData);
        }

        public void UpdateItem(UserData item)
        {
            context.Entry(item).State = EntityState.Modified;
        }
        #endregion
    }
}
