using AutoMapper;
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
    public class UserRepository : IUserRepository
    {
        public async Task AddUserAsync(UserModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var user = new User();
                Mapper.Map(model, user);

                context.Users.Add(user);

                await context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> GetUserAsync(int userId)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
                var model = new UserModel();
                Mapper.Map(user, model);

                return model;
            }
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == username);
                var model = new UserModel();
                Mapper.Map(user, model);

                return model;
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
                return user.Id != editingUser.Id && user != null;
            }
        }

        public async Task UpdateUserAsync(UserModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == model.Id);
                Mapper.Map(model, user);

                context.Users.Add(user);

                await context.SaveChangesAsync();
            }
        }
    }
}
