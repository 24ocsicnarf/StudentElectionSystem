using AutoMapper;
using AutoMapper.QueryableExtensions;
using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.Repositories
{
    public class UserRepository : Repository, IUserRepository
    {
        public async Task AddUserAsync(UserModel model)
        {
            var user = new User();
            _mapper.Map(model, user);

            using (var context = new StudentElectionContext())
            {
                context.Users.Add(user);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(UserModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == model.Id);
                context.Users.Remove(user);

                await context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> GetUserAsync(int userId)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return null;
                }

                var model = new UserModel();
                _mapper.Map(user, model);

                return model;
            }
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                {
                    return null;
                }

                var model = new UserModel();
                _mapper.Map(user, model);

                return model;
            }
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using (var context = new StudentElectionContext())
            {
                var users = context.Users;
                
                return await _mapper.ProjectTo<UserModel>(users)
                    .ToListAsync();
            }
        }

        public async Task<bool> IsUserNameExistingAsync(string username, UserModel editingUser)
        {
            var user = await GetUserByUsernameAsync(username);
            if (editingUser == null)
            {
                return user != null;
            }
            else
            {
                return user != null && user.Id != editingUser.Id;
            }
        }

        public async Task UpdateUserAsync(UserModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == model.Id);
                _mapper.Map(model, user);

                context.Users.Add(user);

                await context.SaveChangesAsync();
            }
        }
    }
}
