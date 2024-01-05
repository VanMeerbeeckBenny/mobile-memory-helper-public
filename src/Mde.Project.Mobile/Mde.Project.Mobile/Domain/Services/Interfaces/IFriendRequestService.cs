using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IFriendRequestService
    {
        Task<ItemResultModel<FriendRequestModel>> GetFriendRequestByUserId(string id);
        Task<ItemResultModel<FriendRequestModel>> UpdateFriendRequest(FriendRequestModel friendRequest);
        Task<ItemResultModel<FriendRequestModel>> AddFriendRequest(FriendRequestModel friendRequest);
    }
}
