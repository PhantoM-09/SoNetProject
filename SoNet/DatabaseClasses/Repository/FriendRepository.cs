using DatabaseClasses.DBContext;
using DatabaseClasses.Repository.Interface;
using Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace DatabaseClasses.Repository
{
    public class FriendRepository : IRepository<Friend>
    {
        #region Fields
        DatabaseContext context;
        #endregion

        #region Ctor
        public FriendRepository()
        {
            this.context = new DatabaseContext();
        }

        public FriendRepository(DatabaseContext context)
        {
            this.context = context;
        }
        #endregion


        #region Methods
        public Friend GetItem(object[] keys)
        {
            return context.Friends.Find((string)keys[0], (string)keys[1]);
        }

        public IEnumerable<Friend> GetItems()
        {
            return context.Friends.AsNoTracking().Include(u=>u.UserInfo).Include(f=>f.FriendInfo).ToList();
        }

        public void AddItem(Friend item)
        {
            if (item != null)
                context.Friends.Add(item);
        }

        public void DeleteItem(object[] keys)
        {
            Friend friend = context.Friends.Find((string)keys[0], (string)keys[1]);
            if (friend != null)
                context.Friends.Remove(friend);
        }

        public void UpdateItem(Friend item)
        {
            context.Entry(item).State = EntityState.Modified;
        }
        #endregion
    }
}
