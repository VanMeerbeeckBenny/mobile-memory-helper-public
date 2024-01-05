using FluentValidation;
using FreshMvvm;
using Mde.Project.Mobile.CustomMasterDetail;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Mde.Project.Mobile.Domain.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.ViewModels
{
    public class LoginViewModel : FreshBasePageModel
    {
        private readonly IAccountService _accountService;
        private readonly IUserLocalInfoService _userLocalInfo;
		private readonly IUserService _userService;
		private readonly IPushNotificationService _pushNotificationService;
		private readonly IsharesService _shareService;
		private readonly IListService _listService;
		private LoginModel currentLogin = new LoginModel();
		private IValidator validation;

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { 

                isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
                RaisePropertyChanged(nameof(ShowContent));
            }
        }

       public bool ShowContent
        {
            get{ return !IsBusy; }
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

        private string emailError;

		public string EmailError
		{
			get { return emailError; }
			set { 
				emailError = value; 
				RaisePropertyChanged(nameof (EmailError));
				RaisePropertyChanged(nameof(EmailErrorVisible));
			}
		}

		public bool EmailErrorVisible
		{
			get { return !string.IsNullOrWhiteSpace(EmailError); }
		}

		private string passwordError;

		public string PasswordError
		{
			get { return passwordError; }
			set {
				passwordError = value;
				RaisePropertyChanged(nameof(PasswordError));
				RaisePropertyChanged(nameof(PasswordErrorVisible));
			}
		}

		public bool PasswordErrorVisible
		{
			get {return !string.IsNullOrWhiteSpace(PasswordError); }
		}


		public LoginViewModel(IAccountService accountService,
							  IUserLocalInfoService userLocalInfo,
                              IUserService userService,
                              IPushNotificationService pushNotificationService,
							  IsharesService shareService,
							  IListService listService)
		{
			_accountService = accountService;
			_userLocalInfo = userLocalInfo;
            _userService = userService;
			_pushNotificationService = pushNotificationService;
			_shareService = shareService;
			_listService = listService;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            validation = new LoginValidation();
            ResetUI();			
        }

        public ICommand RedirectToRegistrationCommand => new Command(
        async () => {
            await CoreMethods.PushPageModel<RegistrationViewModel>();

        });

		public ICommand OnLoginCommand => new Command(
			async() =>
			{
                
				LoadItemState();
				if (Validation(currentLogin))
				{
                    IsBusy = true;
                    var result = await _accountService.Login(currentLogin.Email, currentLogin.Password);
                    if (result.IsSucces)
                    {
                        _userLocalInfo.CreateUserSettings(result.User);
                        await CheckUserPushToken();
                        var masterDetail = Application.Current.MainPage as CustomFreshMasterDetail;
                        await CoreMethods.SwitchSelectedRootPageModel<MainViewModel>();
                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = false;
                        FirebaseError = result.Error;
                    }
                }               
			});
		
		private void LoadItemState()
		{
			currentLogin.Email = Email;
			currentLogin.Password = Password;
		}
		private void ResetUI()
		{			
			Email = string.Empty;
			Password = string.Empty;
			ResetErrors();
		}

		private void ResetErrors()
		{
            PasswordError = string.Empty;
            EmailError = string.Empty;
            FirebaseError = string.Empty;
        }
		private bool Validation(LoginModel login)
		{
			ResetErrors();
            var validationContext = new ValidationContext<LoginModel>(login);
            var validationResult = validation.Validate(validationContext);
            //loop through error to identify properties
            foreach (var error in validationResult.Errors)
            {
                if (error.PropertyName == nameof(login.Email))
                {
                    EmailError = error.ErrorMessage;
                }
				if(error.PropertyName == nameof(login.Password))
				{
					PasswordError = error.ErrorMessage;
				}

            }

            return validationResult.IsValid;
        }

        private async Task CheckUserPushToken()
        {
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
				var currentUser = _userLocalInfo.GetUser();
				var token = await SecureStorage.GetAsync(MyConstants.PushToken);
                if (currentUser != null)
                {
                    var resultUser = await _userService.GetUserByEmail(currentUser.Email);
                    if (resultUser.IsSucces)
                    {
                        var user = resultUser.Items.FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(user.PushToken) || user?.PushToken != token)
                        {                           
                            await RefreshSubscribtions(user.PushToken, token);
                            user.PushToken = token;
                            await _userService.UpdateAsync(user);
                            
							
                        }
                    }
                }
            }
        }

        private async Task RefreshSubscribtions(string oldToken,string newToken)
		{
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {               
                var currentUser = _userLocalInfo.GetUser();
                var userSharesResult = await _shareService.GetSharesByUserId(currentUser.UserId);
                var userOwnedListResult = await _listService.GetListbyUserId(currentUser.UserId);
                if (userSharesResult.IsSucces && userOwnedListResult.IsSucces)
                {
                    var result = await _userService.GetByIdAsync(currentUser.UserId);
                    var ownedLists = userOwnedListResult.Items.Where(i => i.IsShared == true).ToList();
                    var shares = userSharesResult.Items.Where(cs => cs.IsShared == true).ToList();
                    
                    if (result.IsSucces)
                    {
                        foreach (var share in shares)
                        {
                            var currentListResult = await _listService.GetListById(share.ListId);
                            if (currentListResult.IsSucces)
                            {                                                       
                                await _pushNotificationService.Subscribe(currentListResult.Items.FirstOrDefault().Name, new List<string> { newToken });
                            }
                        }
                        foreach (var ownedList in ownedLists)
                        {                            
                            await _pushNotificationService.Subscribe(ownedList.Name, new List<string> { newToken });
                        }
                    }                 
                }

            }
        }

    }
}
