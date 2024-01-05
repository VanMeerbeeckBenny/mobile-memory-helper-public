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
using Xamarin.Forms.Shapes;

namespace Mde.Project.Mobile.Domain.Services.Api
{
    public class FirebaseShareService : IsharesService
    {
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
        public async Task<ItemResultModel<SharedListSettings>> GetSharesByListId(string listId)
        {
            var client = await CreateClient();
            var sharesResult = new ItemResultModel<SharedListSettings>();

            try
            {
                var firebaseResult = await client.Child(nameof(SharedListSettings))
                                                 .OrderBy("ListId")
                                                 .EqualTo(listId)
                                                 .OnceAsync<SharedListSettings>();

                if (firebaseResult.Any())
                {
                    sharesResult.Items = convertToSharedListSettings(firebaseResult);
                    sharesResult.IsSucces = true;
                }
                else sharesResult.Error = "No items!";
            }
            catch (FirebaseException ex)
            {
                sharesResult.Error = ex.InnerException.Message;
            }
            return sharesResult;
        }

        public async Task<ItemResultModel<SharedListSettings>> GetSharesByUserId(string userId)
        {
            var client = await CreateClient();
            var sharesResult = new ItemResultModel<SharedListSettings>();

            try
            {
                var firebaseResult = await client.Child(nameof(SharedListSettings))
                                                 .OrderBy("UserId")
                                                 .EqualTo(userId)
                                                 .OnceAsync<SharedListSettings>();

                if (firebaseResult.Any())
                {
                    var items = firebaseResult.Where(fr => fr.Object.IsShared == true).ToList();
                    if (items.Any())
                    {
                        sharesResult.Items = convertToSharedListSettings(items);
                        sharesResult.IsSucces = true;
                    }
                   
                }
                else sharesResult.Error = "No items!";
            }
            catch (FirebaseException ex)
            {
                sharesResult.Error = ex.InnerException.Message;
            }
            return sharesResult;
        }

        public async Task<ItemResultModel<SharedListSettings>> GetShareSettingsOfListByUserId(string userId, string listId)
        {
            var client = await CreateClient();
            var resultShares = new ItemResultModel<SharedListSettings>();
            try
            {

                var items = await client.Child(nameof(SharedListSettings))
                                        .OrderBy("ListId")
                                        .EqualTo(listId)
                                        .OnceAsync<SharedListSettings>();
                if (items.Any())
                {

                    var foundFirebaseObjects = items.Where(sl => sl.Object.UserId == userId).ToList();
                    var shares = convertToSharedListSettings(foundFirebaseObjects);

                    if (shares.Any())
                    {
                        resultShares.IsSucces = true;
                        resultShares.Items = shares;
                    }
                    else resultShares.Error = "No items";

                }
                else resultShares.Error = "No items";
                return resultShares;
            }
            catch (FirebaseException ex)
            {

                resultShares.Error = ex.InnerException.Message;
                return resultShares;
            }
        }

        public async Task<ItemResultModel<SharedListSettings>> SaveShare(List<SharedListSettings> shares)
        {
            var client = await CreateClient();
            var resultShares = new ItemResultModel<SharedListSettings>();
            try
            {
                foreach (var share in shares)
                {
                    var foundShare = await GetShareSettingsOfListByUserId(share.UserId, share.ListId);
                    if (!foundShare.IsSucces && share.IsShared)                    
                        await client.Child(nameof(SharedListSettings)).PostAsync(share);
                    else if (foundShare.IsSucces)
                    {
                        var items = await client.Child(nameof(SharedListSettings))
                                        .OrderBy("ListId")
                                        .EqualTo(share.ListId)
                                        .OnceAsync<SharedListSettings>();
                        var key = items.FirstOrDefault(i => i.Object.UserId == share.UserId).Key;
                        await client.Child(nameof(SharedListSettings))
                                    .Child(key)
                                    .PutAsync(share);

                    }
                      

                }

                resultShares.IsSucces = true;
                return resultShares;
            }
            catch (FirebaseException ex)
            {

                resultShares.Error = ex.InnerException.Message;
                return resultShares;
            }
        
            
        }
        private List<SharedListSettings> convertToSharedListSettings(IReadOnlyCollection<FirebaseObject<SharedListSettings>> firebaseObjects)
        {
            return firebaseObjects.Select(sl => new SharedListSettings
             {
                 Id = sl.Object.Id,
                 ListId = sl.Object.ListId,
                 OwnerId = sl.Object.OwnerId,
                 UserId = sl.Object.UserId,
                 IsShared = sl.Object.IsShared,
                 Name = sl.Object.Name,
                 WritePermission = sl.Object.WritePermission
             }).ToList();
        }
    }
}
