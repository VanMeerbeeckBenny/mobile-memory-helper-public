using Firebase.Auth;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Api;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace Mde.Project.Mobile.Domain.Services.Local
{
    public class UserLocalInfoService: IUserLocalInfoService
    {
        private string fullPath;

        public UserLocalInfoService()
        {
            fullPath = Path.Combine(FileSystem.AppDataDirectory, MyConstants.UserSettingsFolder);
        }
        public void CreateUserSettings(UserModel user)
        {
            if(user != null)
            {               
                string userSettingsJson = JsonConvert.SerializeObject(user);           

                File.WriteAllText(fullPath, userSettingsJson);               
            }
        }

        public UserModel GetUser()
        {       
            if (File.Exists(fullPath))
            {
                try
                {
                    string JsonUserSettings = File.ReadAllText(fullPath);
                    return JsonConvert.DeserializeObject<UserModel>(JsonUserSettings);
                }
                catch (Exception)
                {
                    return null;                
                }                
            }
            return null;           
        }

      


    }
}
