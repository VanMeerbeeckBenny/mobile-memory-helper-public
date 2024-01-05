using Firebase.Auth;
using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<ItemResultModel<UserModel>> AddUser(UserModel user);
        Task<ItemResultModel<UserModel>> GetUserByEmail(string email);
        Task<ItemResultModel<UserModel>> GetByIdAsync(string id);
        Task<ItemResultModel<UserModel>> UpdateAsync(UserModel userToUpdate);
    }
}
