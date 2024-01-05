using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IListService
    {
        Task<ItemResultModel<ListModel>> GetListbyUserId(string userId);    
        Task<ItemResultModel<ListModel>> GetListById(string listId);
        Task<ItemResultModel<ListModel>> UpdateList(ListModel listToUpdate);
        Task<ItemResultModel<ListModel>> CreateList(ListModel listToCreate);
        Task<ItemResultModel<ListModel>> DeleteList(string id);      
        Task<ItemResultModel<ListModel>> AddItem(ItemModel itemToAdd, string listId);
    }
}
