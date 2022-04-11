using DatabaseClasses.DBContext;
using DatabaseClasses.Repository;
using DatabaseClasses.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseClasses.UnitOfWorkPattern
{
    public class UnitOfWork : IDisposable
    {
        DatabaseContext context = new DatabaseContext();

        private UserDataRepository userDataRepository;
        private FriendRepository friendRepository;
        private PostRepository postRepository;
        private CommentRepository commentRepository;
        private UserFileRepository userFileRepository;

        public UserDataRepository UserDataRepos
        {
            get
            {
                if (userDataRepository == null)
                    userDataRepository = new UserDataRepository(context);
                return userDataRepository;
            }
        }

        public FriendRepository FriendRepos
        {
            get
            {
                if (friendRepository == null)
                    friendRepository = new FriendRepository(context);
                return friendRepository;
            }
        }

        public PostRepository PostRepos
        {
            get
            {
                if (postRepository == null)
                    postRepository = new PostRepository(context);
                return postRepository;
            }
        }

        public CommentRepository CommentRepos
        {
            get
            {
                if (commentRepository == null)
                    commentRepository = new CommentRepository(context);
                return commentRepository;
            }
        }

        public UserFileRepository UserFileRepos
        {
            get
            {
                if (userFileRepository == null)
                    userFileRepository = new UserFileRepository(context);
                return userFileRepository;
            }
        }


        public void Save()
        {
            context.SaveChanges();
        }


        //Dispose-----------------------------------------------
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //-----------------------------------------------------
    }
}
