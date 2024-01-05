using Firebase.Database;
using Firebase.Database.Query;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Mde.Project.Mobile.Domain.Services.Api
{
    public class FirebaseListService : IListService
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
        public async Task<ItemResultModel<ListModel>> AddItem(ItemModel itemToAdd, string listId)
        {
            var result = new ItemResultModel<ListModel>();
            try
            {               
                result = await GetListById(listId);
                if (result.IsSucces)
                {
                    var list = result.Items.FirstOrDefault();
                    list.Items.Add(itemToAdd);
                    result = await UpdateList(list);
                    result.IsSucces = true;
                }
            }
            catch (FirebaseException)
            {
                result.Error = "Something went wrong";
                
            }

            return result;
            
        }

        public async Task<ItemResultModel<ListModel>> CreateList(ListModel listToCreate)
        {
            var client = await CreateClient();
            var resultItem = new ItemResultModel<ListModel>();
            if(listToCreate == null)
            {
                resultItem.Error = "Please provide a valid model";
                return resultItem;
            }
                
            try
            {
                var result = await GetById(listToCreate.Id);
                if (result == null)
                {
                   

                   await client.Child(nameof(ListModel))                                         
                       .PostAsync(listToCreate);
                    
                    resultItem.IsSucces = true;
                }
                
            }
            catch (FirebaseException ex)
            {
                resultItem.Error = ex.InnerException.Message;
            }

            return resultItem;
        }

        public async Task<ItemResultModel<ListModel>> DeleteList(string id)
        {
            var client = await CreateClient();
            var resultModel = new ItemResultModel<ListModel>();
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    
                    string key = result.Key;
                    
                    await client.Child(nameof(ListModel))
                                .Child(key).DeleteAsync();

                    resultModel.IsSucces = true;
                }

            }
            catch (Exception)
            {
                resultModel.Error = "Something went wrong";
                resultModel.IsSucces = false;
            }
            return resultModel;
        }

        public async virtual Task<ItemResultModel<ListModel>> GetListById(string listId)
        {
            var client = await CreateClient();
            var resultModel = new ItemResultModel<ListModel>();
            try
            {
                var result = await client.Child(nameof(ListModel))                    
                    .OrderBy("Id")
                    .EqualTo(listId)
                    .OnceAsync<ListModel>();

                var items = result.Select(x => new ListModel
                {
                    Id = x.Object.Id,
                    Name = x.Object.Name,
                    IsShared = x.Object.IsShared,
                    OwnerId = x.Object.OwnerId,
                    Items = x.Object.Items?.OrderBy(i => i.Name).ToList() ?? new List<ItemModel>()
                }).ToList();
                if(items != null && items.Any())
                {
                    resultModel.Items = items;
                    resultModel.IsSucces = true;
                }              


            }

            catch (FirebaseException ex)
            {
                resultModel.Error = ex.InnerException.Message;
            }

            return resultModel;
        }

        public async Task<ItemResultModel<ListModel>> GetListbyUserId(string userId)
        {
            var client = await CreateClient();
            var resultModel = new ItemResultModel<ListModel>();
            try
            {
               
                var result = await client.Child(nameof(ListModel))
                    .OrderBy("OwnerId")
                    .EqualTo(userId)
                    .OnceAsync<ListModel>();

        
                    
                var items = result.Select(x => new ListModel
                {Id = x.Object.Id,
                    Name = x.Object.Name,
                    IsShared = x.Object.IsShared,
                    OwnerId = x.Object.OwnerId,
                    Items = x.Object.Items?.OrderBy(i => i.Name).ToList() ?? new List<ItemModel>()
                }).ToList();
                if (items != null && items.Any())
                {
                    resultModel.Items = items;
                    resultModel.IsSucces = true;
                }


            }
            
            catch (FirebaseException ex)
            {
                resultModel.Error = ex.InnerException.Message;
            }

            return resultModel;
        }

        public async Task<ItemResultModel<ListModel>> UpdateList(ListModel listToUpdate)
        {
            var client = await CreateClient();
            var resultItem = new ItemResultModel<ListModel>();
            var result = await client.Child(nameof(ListModel))
                .OrderBy("Id")
                .EqualTo(listToUpdate.Id)
                .OnceAsync<ListModel>();
            try
            {
                if (result.Any())
                {
                    var item = result.FirstOrDefault();            
                    await client.Child(nameof(ListModel))
                      .Child(item.Key)
                      .PutAsync(listToUpdate);

                    resultItem.IsSucces = true;
                }
               
            }
            catch (FirebaseException ex)
            {
                resultItem.Error = ex.InnerException.Message;
            }
           
            return resultItem;
        }

        private async Task<FirebaseObject<ListModel>>GetById(string id)
        {
            var client = await CreateClient();
            var result = await client.Child(nameof(ListModel))
                    .OrderBy("Id")
                    .EqualTo(id)
                    .OnceAsync<ListModel>();
            if (result.Any())
                return result.FirstOrDefault();
            else return null;
        }
    }
}
