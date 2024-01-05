using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Mde.Project.Mobile.Domain.Services.Api
{
    public class FirebaseUserService : IUserService
    {
        private async Task<FirebaseClient> CreateClient()
        {
            string token = await SecureStorage.GetAsync(MyConstants.Token);
            FirebaseClient client = new FirebaseClient("firebaseslink",
             new FirebaseOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(token)
             });

            return client;
        }
        public async Task<ItemResultModel<UserModel>> AddUser(UserModel user)
        {
            var client = await CreateClient();
            var resultShares = new ItemResultModel<UserModel>();
            try
            {               
                await client.Child(nameof(UserModel)).PostAsync(user);               

                resultShares.IsSucces = true;
                return resultShares;
            }
            catch (FirebaseException ex)
            {

                resultShares.Error = ex.InnerException.Message;
                return resultShares;
            }
        }

        public async Task<ItemResultModel<UserModel>> GetUserByEmail(string email)
        {
            var client = await CreateClient();
            var resultShares = new ItemResultModel<UserModel>();
            try
            {
                var user = await client.Child(nameof(UserModel))
                            .OrderBy("Email")
                            .EqualTo(email)
                            .OnceAsync<UserModel>();
                if (user.Any())
                {
                    resultShares.Items = user.Select(u => new UserModel
                    {
                        UserId = u.Object.UserId,
                        Name = u.Object.Name,
                        Email = u.Object.Email,
                        PushToken = u.Object.PushToken,
                    });
                    resultShares.IsSucces = true;
                }
                else resultShares.Error = "No user found by this E-mail!";
            }
            catch (FirebaseException ex)
            {

                resultShares.Error = ex.InnerException.Message;               
            }
            return resultShares;

        }

        public async Task<ItemResultModel<UserModel>> UpdateAsync(UserModel userToUpdate)
        {
            var client = await CreateClient();
            string key = await GetKey(userToUpdate.UserId, client);
            var result = new ItemResultModel<UserModel>();
            if (!string.IsNullOrWhiteSpace(key))
            {
                try
                {
                    await client.Child(nameof(UserModel))
                                .Child(key)
                                .PutAsync(userToUpdate);
                    result.IsSucces = true;
                }
                catch (FirebaseException ex)
                {
                    result.Error = ex.InnerException.Message;
                }
            }
            else result.Error = "No user found!";
            return result;
        }

        private async Task<string> GetKey(string userId, FirebaseClient client)
        {
            try
            {
                var foundUser = await client.Child(nameof(UserModel))
                      .OrderBy("UserId")
                      .EqualTo(userId)
                      .OnceAsync<UserModel>();

                if (foundUser.Any())
                {
                    return foundUser.FirstOrDefault().Key;
                }return string.Empty;
            }
            catch (FirebaseException ex)
            {
                return String.Empty;
            }
        }

        public async Task<ItemResultModel<UserModel>> GetByIdAsync(string id)
        {
            var client = await CreateClient();
            var resultShares = new ItemResultModel<UserModel>();
            try
            {
                var user = await client.Child(nameof(UserModel))
                            .OrderBy("UserId")
                            .EqualTo(id)
                            .OnceAsync<UserModel>();
                if (user.Any())
                {
                    resultShares.Items = user.Select(u => new UserModel
                    {
                        UserId = u.Object.UserId,
                        Name = u.Object.Name,
                        Email = u.Object.Email,
                        PushToken = u.Object.PushToken,
                    });
                    resultShares.IsSucces = true;
                }
                else resultShares.Error = "No user found by this Id!";
            }
            catch (FirebaseException ex)
            {

                resultShares.Error = ex.InnerException.Message;
            }
            return resultShares;
        }
    }
}
