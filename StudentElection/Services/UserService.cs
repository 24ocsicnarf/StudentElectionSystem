using Project.Library;
using Project.Library.Helpers;
using Project.Library.Services.Cryptography;
using Project.Library.Services.Cryptography.Interfaces;
using StudentElection.Repository;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections;
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

            if (user != null)
            {
                IHasherService hasherService = new BCryptHasherService();

                if (hasherService.MatchHash(password, user.PasswordHash))
                {
                    return user;
                }
            }
            
            return null;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
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

            if (user.Id == 0)
            {
                IHasherService hasherService = new BCryptHasherService();

                user.PasswordHash = hasherService.GetHash(password);
                await _userRepository.AddUserAsync(user);
            }
            else
            {
                //TODO: CHANGE PASSWORD
                await _userRepository.UpdateUserAsync(user);
            }
        }

        public async Task<int> CountUsersAsync()
        {
            return await _userRepository.CountUsersAsync();
        }

        public async Task DeleteUserAsync(UserModel user)
        {
            await _userRepository.DeleteUserAsync(user);
        }
    }
}
