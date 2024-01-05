using Firebase.Auth;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Mde.Project.Mobile.Domain.Services.Api
{
    public class AccountService : IAccountService
    {
        const string apikey = "apiKey"; 
        private readonly FirebaseAuthProvider firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig(apikey));
        public async Task<AuthenticationResultModel> CreateAccount(string userName, string email, string password)
        {
            var resultModel = new AuthenticationResultModel();
            try
            {
                var token = await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password,userName,true);
                if (string.IsNullOrEmpty(token.FirebaseToken))
                {

                    resultModel.Error = "Something whent wrong!!";
                }

                UserModel user = new UserModel
                {   
                    UserId = token.User.LocalId,
                    Email = token.User.Email,
                    Name = token.User.DisplayName
                };

                resultModel.Token = token.FirebaseToken;             
                resultModel.IsSucces = true;
                resultModel.User = user;

                return resultModel;
            }
            catch (FirebaseAuthException ex)
            {
                resultModel.Error = ex.Reason.ToString();
                return resultModel;
            }
        }

        public async Task<AuthenticationResultModel> GetUser(string token)
        {
            try
            {
                User user = await firebaseAuth.GetUserAsync(token);
                UserModel currentUser = new UserModel
                {
                    Email = user.Email,
                    Name = user.DisplayName,
                    UserId = user.LocalId
                };

                return new AuthenticationResultModel
                {
                    IsSucces = true,
                    User = currentUser,
                    Token = token
                };
            }
            catch (Exception)
            {

                return new AuthenticationResultModel { Error = "User not found!" };
            }   
          
        }

        public async Task<AuthenticationResultModel> Login(string email,string password)
        {
            var result = new AuthenticationResultModel();
            UserModel loggedInUser = new UserModel();
            try
            {
                await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        result.Error = "Something went wrong,try again later";
                    }
                    if (task.IsFaulted)
                    {
                        result.Error = "Something went wrong,try again later";
                    }

                    if (task.IsCompleted)
                    {
                        if (task.Result.User.IsEmailVerified)
                        {
                            loggedInUser.UserId = task.Result.User.LocalId;
                            loggedInUser.Email = task.Result.User.Email;
                            loggedInUser.Name = task.Result.User.DisplayName;

                            result.User = loggedInUser;
                            result.IsSucces = true;                            

                            string token = task.Result.FirebaseToken;
                            result.Token = token;
                            SecureStorage.SetAsync(MyConstants.Token, token);
                        }
                        result.Error = "Please verify your account!";
                    }


                });
            }
            catch (Exception ex)
            {

                var exception = ex.InnerException as FirebaseAuthException;
                if (exception == null) result.Error = ex.Message;
                else
                {
                    string error = exception.Reason.ToString();
                    result.Error = error.Contains("Password") || error.Contains("Email") ? "E-mail and password don't match!" : error;
                }
            }
            

            return result;
        }
    }
}
