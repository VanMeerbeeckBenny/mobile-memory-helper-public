using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IUserLocalInfoService
    {
        void CreateUserSettings(UserModel user);
        UserModel GetUser();
    }
}
