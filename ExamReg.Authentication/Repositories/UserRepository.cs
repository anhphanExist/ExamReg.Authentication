using ExamReg.Authentication.Entities;
using ExamReg.Authentication.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamReg.Authentication.Repositories
{
    public interface IUserRepository
    {
        Task<int> Count(UserFilter filter);
        Task<User> Get(UserFilter filter);
        Task<bool> Create(User user);
        Task<bool> Update(User user);
    }
    public class UserRepository : IUserRepository
    {
        private ExamRegContext examRegContext;
        public UserRepository(ExamRegContext examRegContext)
        {
            this.examRegContext = examRegContext;
        }

        public async Task<int> Count(UserFilter filter)
        {
            IQueryable<UserDAO> users = examRegContext.User.AsNoTracking();
            users = DynamicFilter(users, filter);
            return await users.CountAsync();
        }

        public async Task<bool> Create(User user)
        {
            examRegContext.User.Add(new UserDAO
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                IsAdmin = user.IsAdmin,
                StudentId = user.StudentId
            });
            await examRegContext.SaveChangesAsync();
            return true;
        }

        public async Task<User> Get(UserFilter filter)
        {
            IQueryable<UserDAO> users = examRegContext.User.AsNoTracking();
            UserDAO userDAO = DynamicFilter(users, filter).FirstOrDefault();
            return new User
            {
                Id = userDAO.Id,
                Password = userDAO.Password,
                Username = userDAO.Username,
                IsAdmin = userDAO.IsAdmin,
                StudentId = userDAO.StudentId,
                StudentNumber = !userDAO.IsAdmin ? userDAO.Student.StudentNumber : 0
            };
        }

        public async Task<bool> Update(User user)
        {
            examRegContext.User
                .Where(u => u.Id.Equals(user.Id))
                .UpdateFromQuery(u => new UserDAO
                {
                    Password = user.Password
                });
            await examRegContext.SaveChangesAsync();
            return true;
        }

        private IQueryable<UserDAO> DynamicFilter(IQueryable<UserDAO> query, UserFilter filter)
        {
            if (filter == null)
                return query.Where(q => 1 == 0);
            if (filter.Username != null)
                query = query.Where(u => u.Username.Equals(filter.Username));
            if (filter.Password != null)
                query = query.Where(u => u.Password.Equals(filter.Password));
            if (filter.StudentId != null)
                query = query.Where(u => u.StudentId.Equals(filter.StudentId));
            return query;
        }
    }
}
