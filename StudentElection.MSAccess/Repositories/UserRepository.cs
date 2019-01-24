using AutoMapper;
using AutoMapper.QueryableExtensions;
using StudentElection.MSAccess.StudentElectionDataSetTableAdapters;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentElection.MSAccess.StudentElectionDataSet;

namespace StudentElection.MSAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task AddUserAsync(UserModel user)
        {
            using (var tableAdapter = new UserTableAdapter())
            {
                await Task.CompletedTask;

                tableAdapter.Insert(
                    user.FirstName,
                    user.MiddleName,
                    user.LastName,
                    user.Suffix,
                    user.Sex,
                    (int)user.Type,
                    user.UserName,
                    user.PasswordHash
                );
            }
        }

        public async Task DeleteUserAsync(UserModel user)
        {
            using (var tableAdapter = new UserTableAdapter())
            {
                await Task.CompletedTask;

                tableAdapter.Delete(
                    user.Id
                );
            }
        }

        public async Task<UserModel> GetUserAsync(int userId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new UserTableAdapter())
            {
                var row = tableAdapter.GetUser(userId).FirstOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new UserModel();
                Mapper.Map(row, model);

                return model;
            }
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new UserTableAdapter())
            {
                var row = tableAdapter.GetUsersByUsername(username).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }
                if (!row.UserName.Equals(username, StringComparison.Ordinal))
                {
                    return null;
                }

                var model = new UserModel();
                Mapper.Map(row, model);

                return model;
            }
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            await Task.CompletedTask;

            using (var tableAdapter = new UserTableAdapter())
            {
                return tableAdapter.GetData()
                    .AsQueryable()
                    .ProjectTo<UserModel>()
                    .AsEnumerable();
            }
        }

        public async Task<bool> IsUserNameExistingAsync(string username, UserModel editingUser)
        {
            var row = await GetUserByUsernameAsync(username);

            if (editingUser == null)
            {
                return row != null;
            }
            else
            {
                return row != null && row.Id != editingUser.Id;
            }
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            using (var tableAdapter = new UserTableAdapter())
            {
                await Task.CompletedTask;

                tableAdapter.Update(
                    user.FirstName,
                    user.MiddleName,
                    user.LastName,
                    user.Suffix,
                    user.Sex,
                    (int)user.Type,
                    user.UserName,
                    user.PasswordHash,
                    user.Id
                );
            }
        }
    }
}
