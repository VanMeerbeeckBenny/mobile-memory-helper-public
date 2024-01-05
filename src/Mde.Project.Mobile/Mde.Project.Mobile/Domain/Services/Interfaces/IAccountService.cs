using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AuthenticationResultModel> CreateAccount(string userName,string email, string password);
        Task<AuthenticationResultModel> GetUser(string token);
        Task<AuthenticationResultModel> Login(string email,string password);
    }
}
