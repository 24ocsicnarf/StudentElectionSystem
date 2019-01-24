using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsUserNameExistingAsync(string username, UserModel editingUser);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<UserModel> GetUserAsync(int userId);
        Task AddUserAsync(UserModel user);
        Task UpdateUserAsync(UserModel user);
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task DeleteUserAsync(UserModel user);
    }
}
