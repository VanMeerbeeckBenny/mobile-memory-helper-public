using FluentValidation;
using FreshMvvm;
using Mde.Project.Mobile.CustomMasterDetail;
using Mde.Project.Mobile.Domain.Helpers;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Mde.Project.Mobile.Domain.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.ViewModels
{
    public class RegistrationViewModel : FreshBasePageModel
    {
        private IValidator registrationValidator;
        private RegistrationModel currentRegistration;     
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        #region Properties

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { 
                userName = value;
                RaisePropertyChanged(nameof(UserName));
            }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { 
                email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set {
                password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        private string verifyPassword;

        public string VerifyPassword
        {
            get { return verifyPassword; }
            set
            {
                verifyPassword = value;
                RaisePropertyChanged(nameof(VerifyPassword));
            }
        }

        private string firebaseError;

        public string FirebaseError
        {
            get { return firebaseError; }
            set
            {
                firebaseError = value;
                RaisePropertyChanged(nameof(FirebaseError));
                RaisePropertyChanged(nameof(FirebaseErrorVisible));
            }
        }

        public bool FirebaseErrorVisible
        {
            get { return !string.IsNullOrWhiteSpace(FirebaseError); }
        }

        private string verifyPasswordError;

        public string VerifyPasswordError
        {
            get { return verifyPasswordError; }
            set
            {
                verifyPasswordError = value;
                RaisePropertyChanged(nameof(VerifyPasswordError));
                RaisePropertyChanged(nameof(VerifyPasswordErrorVisible));
            }
        }

        public bool VerifyPasswordErrorVisible
        {
            get { return !string.IsNullOrWhiteSpace(VerifyPasswordError); }
        }

        private string passwordError;

        public string PasswordError
        {
            get { return passwordError; }
            set
            {
                passwordError = value;
                RaisePropertyChanged(nameof(PasswordError));
                RaisePropertyChanged(nameof(PasswordErrorVisible));
            }
        }
        public bool PasswordErrorVisible
        {
            get { return !string.IsNullOrWhiteSpace(PasswordError); }
        }

        private string userNameError;

        public string UserNameError
        {
            get { return userNameError; }
            set {
                userNameError = value;
                RaisePropertyChanged(nameof(UserNameError));
                RaisePropertyChanged(nameof(UserNameErrorVisible));
            }
        }
        public bool UserNameErrorVisible
        {
            get { return !string.IsNullOrWhiteSpace(UserNameError); }
        }


        private string emailError;
        public string EmailError
        {
            get { return emailError; }
            set {
                emailError = value;
                RaisePropertyChanged(nameof(EmailError));
                RaisePropertyChanged(nameof(EmailErrorVisible));
            }
        }
        public bool EmailErrorVisible
        {
            get { return !string.IsNullOrWhiteSpace(EmailError); }
        }

        #endregion

        public RegistrationViewModel(IAccountService accountService,IUserService userService)
        {            
            _accountService = accountService;
            _userService = userService;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            currentRegistration = new RegistrationModel();
            registrationValidator = new RegistrationValidator();
            ResetUI();            
        }

        public ICommand RedirectToLoginCommand => new Command(
           async () => {             
                   await CoreMethods.PushPageModel<LoginViewModel>();
              
            });

        public ICommand CreateAccountCommand => new Command(
         async () => {
             try
             {
                 CreateCurrentRegistration();
                 if (Validate(currentRegistration))
                 {
                     var result = await _accountService.CreateAccount(currentRegistration.UserName,
                                                                currentRegistration.Email,
                                                                currentRegistration.Password);
                     var userTableResult = await _userService.AddUser(result.User);
                     if (result.IsSucces && userTableResult.IsSucces)await CoreMethods.PopPageModel();
                     else FirebaseError = result.Error;
                 }
             }
             catch (Exception)
             {

                 FirebaseError = "Something went wrong";
             }

         });

        private void CreateCurrentRegistration()
        {
            currentRegistration.UserName = UserName;
            currentRegistration.Email = Email;
            currentRegistration.Password = Password;
            currentRegistration.VerifyPassword = VerifyPassword;
        }

        private void ResetUI()
        {
            Email = string.Empty;
            Password = string.Empty;
            verifyPassword = string.Empty;
            UserName = string.Empty;
            ResetErrors();
        }

        private void ResetErrors()
        {
            UserNameError = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            VerifyPasswordError = string.Empty;           
            FirebaseError = string.Empty;
        }

        private bool Validate(RegistrationModel registration)
        {
            ResetErrors();
            var validationContext = new ValidationContext<RegistrationModel>(registration);
            var validationResult = registrationValidator.Validate(validationContext);
            //loop through error to identify properties
            foreach (var error in validationResult.Errors)
            {
                if (error.PropertyName == nameof(registration.Email))
                {
                    EmailError = error.ErrorMessage;
                }
                if (error.PropertyName == nameof(registration.UserName))
                {
                    UserNameError = error.ErrorMessage;
                }
                if(error.PropertyName == nameof(registration.Password))
                {
                    PasswordError = error.ErrorMessage;
                }
                if(error.PropertyName == nameof(registration.VerifyPassword))
                {
                    VerifyPasswordError = error.ErrorMessage;
                }
            }

            return validationResult.IsValid;
        }
    }
}
