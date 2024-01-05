using Firebase.Auth;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Mde.Project.Mobile.Domain.Services.Mocking
{
    //This is able to do witg SDK admin firebase
    //As this is complex and i stil have a lot to do i create a user table in the database
    public class MockingUserService : IUserService
    {
        List<UserModel> _users = new List<UserModel>
        {
            new UserModel{UserId="PGgBqu411YXdsqdsqdg1nGgEEg1" , Name="Joske",Email="joske@hotmail.com"},
            new UserModel{UserId="PGgBqu411YXds157dg1nGgEEg1" , Name="Siefried",Email="siefried@hotmail.com"},
            new UserModel{UserId="PGgBqud5sq4s5s7sqdg1nGgEEg1" , Name="Jurgen",Email="jurgen@hotmail.com"},
            new UserModel{UserId="15s5u411YXds157dg1nGgEEg1" , Name="Wesly",Email="wesly@hotmail.com"},
            new UserModel{UserId="h5f6f8y5sq4s5s7sqdg1nGgEEg1" , Name="Yordi",Email="yordi@hotmail.com"}

        };


        public async Task<ItemResultModel<UserModel>>AddUser(UserModel user)
        {
            var result = new ItemResultModel<UserModel>();
            var foundUser = _users.FirstOrDefault(u => u.Email == user.Email);

            if (foundUser != null)
            {
                _users.Add(user);
                result.IsSucces = true;
            }
            else result.Error = "User already exists!";

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<UserModel>> GetByIdAsync(string id)
        {
            var result = new ItemResultModel<UserModel>();
            var foundUser = _users.FirstOrDefault(u => u.UserId == id);

            if (foundUser != null)
            {
                result.Items = new List<UserModel> { foundUser };
                result.IsSucces = true;
            }
            else result.Error = "No user found with that E-mail!";

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<UserModel>> GetUserByEmail(string email)
        {
            var result = new ItemResultModel<UserModel>();
            var foundUser = _users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (foundUser != null)
            {
                result.Items = new List<UserModel> { foundUser };
                result.IsSucces = true;
            }
            else result.Error = "No user found with that E-mail!";

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<UserModel>> UpdateAsync(UserModel userToUpdate)
        {
            var result = new ItemResultModel<UserModel>();
            var foundUser = _users.FirstOrDefault(u => u.Email == userToUpdate.Email);

            if (foundUser != null)
            {
                _users.Remove(foundUser);
                _users.Add(userToUpdate);
                result.IsSucces = true;
            }
            else result.Error = "User already exists!";

            return await Task.FromResult(result);
        }
    }
}
