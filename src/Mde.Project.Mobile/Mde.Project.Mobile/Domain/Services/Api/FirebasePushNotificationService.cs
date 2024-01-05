using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoreImage;
using FirebaseAdmin;

using Foundation;
using Google.Apis.Auth.OAuth2;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Plugin.FirebasePushNotification;
using Xamarin.Essentials;
using Mde.Project.Mobile;
using static Google.Apis.Requests.BatchRequest;
using FirebaseAdmin.Messaging;

namespace Mde.Project.Mobile.Domain.Services.Api
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        public async Task SendMessage(string title,string message,string topic)
        {
            try
            {
                if (FirebaseMessaging.DefaultInstance == null)
                {
                    await CreateApp();
                }

                var pushNotification = new Message()
                {
                    //Data = new Dictionary<string, string>()
                    //    {
                    //        { "myData", "1337" },
                    //    },
                    //Token = registrationToken,
                    Topic = topic,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = message
                    }
                };

                // Send a message to the device corresponding to the provided
                // registration token.
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(pushNotification);
                Console.WriteLine($"{response} tokens were subscribed successfully");
            }
            catch (FirebaseException ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
          
           
        }

        public string GetPushToken()
        {
            return CrossFirebasePushNotification.Current.Token;
        }

        public async Task<bool> Subscribe(string topic, List<string> tokens)
        {
            try
            {
                if (FirebaseMessaging.DefaultInstance == null)
                {
                    await CreateApp();
                }                
                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(tokens,topic.Replace(" ",""));
          
                return true;
            }
            catch (Exception ex)
            {

                string error = ex.InnerException.Message;
                return false;
            }
          
        }

        public async Task<bool> SendToUser(UserModel sender, string token)
        {
            try
            {
                if (FirebaseMessaging.DefaultInstance == null)
                {
                    await CreateApp();
                }
                var message = new Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Title = "New Friend Request!",
                        Body = $"{sender.Name} has send you a friendRequest!"
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);        
                return true;
            }
            catch (Exception)
            {               
                return false;
            }

        }


        public async Task<bool> UnSubscribe(string topic, List<string> tokens)
        {
            try
            {
                if (FirebaseMessaging.DefaultInstance == null)
                {
                    await CreateApp();
                }
                var response = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(tokens, topic.Replace(" ", ""));             
                return true;
            }
            catch (Exception)
            {                
                return false;
            }

        }

        private static async Task CreateApp()
        {
            var jsonString = "";
            var assembly = typeof(FirebasePushNotificationService).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Private_key.json");
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonString = await reader.ReadToEndAsync();
            }

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(jsonString)
            });
        }
    }
}
