using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IFriendsService
    {
        Task<ItemResultModel<UserModel>>GetFriendsAsyncByUserId(string userId);
        Task<ItemResultModel<UserModel>> AddFriend(string userId, UserModel friend);
        Task<ItemResultModel<UserModel>> RemoveFriend(string userId, UserModel friend);
    }
}
