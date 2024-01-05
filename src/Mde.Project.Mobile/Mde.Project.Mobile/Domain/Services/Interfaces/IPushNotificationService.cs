using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mde.Project.Mobile.Domain.Models;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendMessage(string title, string message, string topic);

        string GetPushToken();

        Task<bool> Subscribe(string topic, List<string> tokens);
        Task<bool> UnSubscribe(string topic, List<string> tokens);
        Task<bool> SendToUser(UserModel sender, string token);
    }
}
