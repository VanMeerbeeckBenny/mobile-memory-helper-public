using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IsharesService
    {
        Task<ItemResultModel<SharedListSettings>> GetSharesByListId(string listId);
        Task<ItemResultModel<SharedListSettings>> GetSharesByUserId(string userId);
        Task<ItemResultModel<SharedListSettings>> SaveShare(List<SharedListSettings> shares);
        Task<ItemResultModel<SharedListSettings>> GetShareSettingsOfListByUserId(string userId, string listId);
    }
}
