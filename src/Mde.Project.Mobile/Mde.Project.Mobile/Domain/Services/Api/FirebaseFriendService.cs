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
    public class FirebaseFriendService : IFriendsService
    {
        private const string NoFriendsModelError = "No friends to show!";
        private async Task<FirebaseClient>CreateClient()
        {
            string token = await SecureStorage.GetAsync(MyConstants.Token);
            FirebaseClient client = new FirebaseClient("firebaselink",
             new FirebaseOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(token)
             });

            return client;
        }
        public async Task<ItemResultModel<UserModel>> AddFriend(string userId, UserModel friend)
        {
            var userResult = new ItemResultModel<UserModel>();
            var client = await CreateClient();
            if (friend != null)
            {
                try
                {
                    var foundFriendResult = await GetFriendsAsyncByUserId(userId);
                    if (foundFriendResult.IsSucces)
                    {
                        var allFriends = foundFriendResult.Items.ToList();
                        var foundFriend = allFriends.FirstOrDefault(ffr => ffr.UserId == friend.UserId);
                        if (foundFriend == null)
                        {
                            string key = await GetPrimaryKey(userId, client);
                            allFriends.Add(friend);
                            FriendsModel friendsModel = new FriendsModel { Friends = allFriends, UserId = userId };
                            await client.Child(nameof(FriendsModel))
                                        .Child(key)
                                        .PutAsync(friendsModel);                            
                            userResult.IsSucces = true;
                        }
                    }
                    else if (foundFriendResult.Error.Contains(NoFriendsModelError))
                    {

                        FriendsModel friendsModel = new FriendsModel { Friends = new List<UserModel> { friend }, UserId = userId };
                        await client.Child(nameof(FriendsModel))
                                    .PostAsync(friendsModel);
                        userResult.IsSucces = true;
                    }
                    else userResult.Error = foundFriendResult.Error;
                }
                catch (FirebaseException ex)
                {
                    userResult.Error = ex.InnerException.Message;
                }

            }
            else userResult.Error = "Please provide a friend!";
            return userResult;
        }

        public async Task<ItemResultModel<UserModel>> GetFriendsAsyncByUserId(string userId)
        {
            var userResult = new ItemResultModel<UserModel>();
            var client = await CreateClient();           
            try
            {
                var foundResult = await client.Child(nameof(FriendsModel))
                                        .OrderBy("UserId")
                                        .EqualTo(userId)
                                        .OnceAsync<FriendsModel>();
                if (foundResult.Any())
                {
                    var friendsResult = foundResult.Select(fr => fr.Object).FirstOrDefault();                   
                    userResult.Items = friendsResult?.Friends ?? new List<UserModel>();
                    userResult.IsSucces = true;                 
                }
                else userResult.Error = NoFriendsModelError;                
                    
            }
            catch (FirebaseException ex)
            {
                userResult.Error = ex.InnerException.Message;                
            }
            return userResult;
        }

        public async Task<ItemResultModel<UserModel>> RemoveFriend(string userId, UserModel friend)
        {
            var client = await CreateClient();
            var result = new ItemResultModel<UserModel>();
            if (friend != null)
            {
                try
                {
                    var foundResult = await GetFriendsAsyncByUserId(userId);
                    if (foundResult.IsSucces)
                    {
                        var friends = foundResult.Items.ToList();
                        var foundFriend = friends.FirstOrDefault(f => f.UserId == friend.UserId);
                        friends.Remove(foundFriend);
                        string key = await GetPrimaryKey(userId, client);
                        var friendsModel = new FriendsModel { Friends = friends, UserId = userId };
                        await client.Child(nameof(FriendsModel))
                                    .Child(key)
                                    .PutAsync(friendsModel);

                        result.IsSucces = true;
                    }
                    else result.Error = "No sutch friend!";
                }
                catch (FirebaseException ex)
                {
                    result.Error = ex.InnerException.Message;
                }
            }
            else result.Error = "Please provide a valid friend!";

            return result;
        }

        private async Task<string> GetPrimaryKey(string userId, FirebaseClient client)
        {
            string key = "";
            try
            {
                var foundFriendsList = await client.Child(nameof(FriendsModel))
                                        .OrderBy("UserId")
                                        .EqualTo(userId)
                                        .OnceAsync<FriendsModel>();
                key = foundFriendsList.FirstOrDefault().Key;
            }
            catch (FirebaseException ex)
            {
               
            }

            return key;
        }
    }
}
