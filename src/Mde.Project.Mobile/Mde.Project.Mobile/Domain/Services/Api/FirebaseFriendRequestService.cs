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
    public class FirebaseFriendRequestService:IFriendRequestService
    {
        public async Task<ItemResultModel<FriendRequestModel>> AddFriendRequest(FriendRequestModel friendRequest)
        {
            var friendResult = new ItemResultModel<FriendRequestModel>();
            var client = await CreateClient();
            try
            {                
                var result = await GetFriendRequestByUserId(friendRequest.UserId);
                if (result.IsSucces)
                {
                    var foundItem = result.Items.FirstOrDefault(fr => fr.FriendId == friendRequest.FriendId);
                    if (foundItem != null)
                    {
                        friendResult.Error = "Request exists and is stil awaited!";
                        return friendResult;
                    }
                }
                await client.Child(nameof(FriendRequestModel)).PostAsync<FriendRequestModel>(friendRequest);
                friendResult.IsSucces = true;
                return friendResult;
            }
            catch (FirebaseException ex)
            {
                friendResult.Error = ex.InnerException.Message;
                return friendResult;
            }
        }

        public async Task<ItemResultModel<FriendRequestModel>> GetFriendRequestByUserId(string id)
        {
            var friendResult = new ItemResultModel<FriendRequestModel>();
            var client = await CreateClient();
            try
            {
                var friendRequest = await client.Child(nameof(FriendRequestModel))
                            .OrderBy("UserId")
                            .EqualTo(id)
                            .OnceAsync<FriendRequestModel>();

                friendResult.Items = friendRequest.Where(fr => fr.Object.IsRejected == false &&
                                                               fr.Object.IsAccepted == false)
                                                  .Select(fr => new FriendRequestModel
                {
                    Id = fr.Object.Id,
                    FriendId = fr.Object.FriendId,
                    UserId = fr.Object.UserId,
                    FriendName = fr.Object.FriendName,
                    Email = fr.Object.Email,
                    IsAccepted = fr.Object.IsAccepted,
                    IsRejected = fr.Object.IsRejected
                });

                friendResult.IsSucces = true;
                return friendResult;
            }
            catch (FirebaseException ex)
            {
                friendResult.Error = ex.InnerException.Message;
                return friendResult;
            }
        }

        public async Task<ItemResultModel<FriendRequestModel>> UpdateFriendRequest(FriendRequestModel friendRequest)
        {
            var friendResult = new ItemResultModel<FriendRequestModel>();
            var client = await CreateClient();        


            try
            {
                var getRequestResult = await client.Child(nameof(FriendRequestModel))
                                   .OrderBy("Id")
                                   .EqualTo(friendRequest.Id)
                                   .OnceAsync<FriendRequestModel>();
              
                if(getRequestResult.Any())
                {
                    var item = getRequestResult.FirstOrDefault();
                    await client.Child(nameof(FriendRequestModel))
                                .Child(item.Key)
                                .PutAsync<FriendRequestModel>(friendRequest);
                    friendResult.IsSucces = true;                    
                }
            
            }
            catch (FirebaseException ex)
            {
                friendResult.Error = ex.InnerException.Message;                
            }
            return friendResult;
        }

        private async Task<FirebaseClient> CreateClient()
        {
            string token = await SecureStorage.GetAsync(MyConstants.Token);
            FirebaseClient client = new FirebaseClient("firebaselink",
             new FirebaseOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(token)
             });

            return client;
        }
    }
}
