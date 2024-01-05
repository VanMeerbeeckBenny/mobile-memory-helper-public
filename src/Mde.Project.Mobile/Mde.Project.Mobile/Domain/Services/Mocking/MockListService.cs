using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Mocking
{
    public class MockListService : IListService
    {
        private static List<ListModel> _mockLists = new List<ListModel>
        {
            new ListModel
            {
                Id="PXgBqu411YX0JG6MAcOg1nGgEEg1",
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                Name="My cars",
                Items = new List<ItemModel>
                {
                    new ItemModel{ ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1",Id = "xxx4",Name="Bugatti",Description="Fast car"},
                    new ItemModel{ ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1", Id = "xxx5",Name="Lada",Description="Slow car"},
                    new ItemModel{ ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1", Id = "xxx6",Name="Lamborgini",Description="Beautifull car"},
                },
            },

            new ListModel
            {
                Id = "xXgBqu411YX0JG6MAcOg1nGgEEg1",
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                Name="My shopping list",
                Items = new List<ItemModel>
                {
                    new ItemModel{ListId="xXgBqu411YX0JG6MAcOg1nGgEEg1", Id = "xxx", Name="cola"},
                    new ItemModel{ListId="xXgBqu411YX0JG6MAcOg1nGgEEg1", Id = "xxx1",Name="Fanta",Description="The real one not fake"},
                    new ItemModel{ListId="xXgBqu411YX0JG6MAcOg1nGgEEg1", Id = "xxx2",Name="salami"},
                },            
            },
             new ListModel
            {
                Id = "xXgBqu411YX0JG6MAcOg1nGgEEg15",
                OwnerId = "sdzru411YX0JG6MAcOg1sgeEEg1",
                Name="Dogs",
                Items = new List<ItemModel>
                {
                    new ItemModel{ListId="xXgBqu411YX0JG6MAcOg1nGgEE15", Id = "xx12x", Name="Duitse herder"},
                    new ItemModel{ListId="xXgBqu411YX0JG6MAcOg1nGgEE15", Id = "xxx1x",Name="Borde Collie",Description="What a beauty"},
                    new ItemModel{ListId="xXgBqu411YX0JG6MAcOg1nGgEE15", Id = "xxx2x",Name="Mechelaar"},
                },
            },
             new ListModel
            {
                Id="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                Name="My cocktails",              
                Items = new List<ItemModel>
                {
                    new ItemModel{ListId = "XxxBqu411YX0JG6MAcOg1nGgEEg1",  Id = "1xxx",Name="Pina colada",Description="Taste awsome"},
                    new ItemModel{ListId = "XxxBqu411YX0JG6MAcOg1nGgEEg1", Id = "2xxx",Name="Gin tonic",Description="verry good"},
                    new ItemModel{ListId = "XxxBqu411YX0JG6MAcOg1nGgEEg1", Id = "3xxx",Name="Tequila sunrise",Description="Not bad"},
                },              
            },
        };    

        public async Task<ItemResultModel<ListModel>> GetListbyUserId(string userId)
        {
            var foundList = _mockLists.Where(l => l.OwnerId == userId).ToList();
            var result = new ItemResultModel<ListModel>
            {
                Items = foundList,
                IsSucces = true
            };

            return await Task.FromResult(result);
        }
        
        public async Task<ItemResultModel<ListModel>> GetListDetailsByUserId(string userId)
        {
            var foundList = _mockLists.Where(l => l.OwnerId == userId).ToList();
            var result = new ItemResultModel<ListModel>
            {
                Items = foundList,
                IsSucces = true
            };

            return await Task.FromResult(result);           
        }

        public async Task<ItemResultModel<ListModel>> GetListById(string listId)
        {
            var foundList = _mockLists.FirstOrDefault(l => l.Id == listId);
            if (foundList == null) { return new ItemResultModel<ListModel>(); }
            var result = new ItemResultModel<ListModel>
            {
                Items = new List<ListModel> { foundList },
                IsSucces = true
            };

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<ListModel>> UpdateList(ListModel listToUpdate)
        {
            var foundList = _mockLists.FirstOrDefault(l => l.Id == listToUpdate.Id);
            var result = new ItemResultModel<ListModel>();

            if (foundList != null)
            {
               
                _mockLists.Remove(listToUpdate);
                _mockLists.Add(listToUpdate);
                result.IsSucces = true;
            }
            else result.Error = "Someting went wrong!!";
           
            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<ListModel>> CreateList(ListModel listToCreate)
        {
            var result = new ItemResultModel<ListModel>();
            if (listToCreate != null)
            {
                listToCreate.Id = Guid.NewGuid().ToString();
                listToCreate.Items = new List<ItemModel>();
                _mockLists.Add(listToCreate);
                result.IsSucces = true;
            }
            else result.Error = "Please provide a valid list";
              

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<ListModel>> DeleteList(string id)
        {
            var result = new ItemResultModel<ListModel>();

            var listToDelete =  _mockLists.FirstOrDefault(l => l.Id == id);
            if (listToDelete != null)
            {
                _mockLists.Remove(listToDelete);
                result.IsSucces = true;
            }
            else
            {
                result.Error = "Item not found!";
            }

            return await Task.FromResult(result);
        }   

        public async Task<ItemResultModel<ListModel>> AddItem(ItemModel itemToAdd, string listId)
        {

            var result = await GetListById(listId);
            if (!result.IsSucces) return new ItemResultModel<ListModel> { Error = result.Error };
            var foundList = result.Items.FirstOrDefault();
            var foundItem = foundList.Items.FirstOrDefault(item => item.Id == itemToAdd.Id);

            if (foundItem == null)
            {
                return new ItemResultModel<ListModel>
                {
                    Error = "Item not found"
                };
            }

            foundList.Items.Remove(foundItem);
            foundList.Items.Add(itemToAdd);
            var resultUpdate = await UpdateList(foundList);
            if (!resultUpdate.IsSucces) return new ItemResultModel<ListModel> { Error = resultUpdate.Error };
            return new ItemResultModel<ListModel> { IsSucces = true };

        }

    }
}
