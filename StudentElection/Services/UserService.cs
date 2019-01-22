using Project.Library;
using Project.Library.Helpers;
using StudentElection.Repository;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService()
        {
            _userRepository = RepositoryFactory.Get<IUserRepository>();
        }

        public async Task<UserModel> LogInAsync(string username, SecureString password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (await CryptographyHelper.MatchHashAsync(password, user.PasswordHash))
            {
                return user;
            }
            
            return null;
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<UserModel> GetUserAsync(int userId)
        {
            return await _userRepository.GetUserAsync(userId);
        }
        
        public async Task<bool> IsUserNameExistingAsync(string username, UserModel editingUser)
        {
            return await _userRepository.IsUserNameExistingAsync(username, editingUser);
        }

        public async Task SaveUserAsync(UserModel user, SecureString password)
        {
            //TODO: Validations here
            System.Diagnostics.Contracts.Contract.Requires<ArgumentNullException>(user.FirstName.IsBlank(), "First name is required");

            if (user.Id == 0)
            {
                user.PasswordHash = await CryptographyHelper.GetHashAsync(password);
                await _userRepository.AddUserAsync(user);
            }
            else
            {
                //TODO: CHANGE PASSWORD
                await _userRepository.UpdateUserAsync(user);
            }
        }
    }
}
