using FluentValidation;
using FreshMvvm;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Mde.Project.Mobile.Domain.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mde.Project.Mobile.ViewModels
{
    public class AddFriendViewModel:FreshBasePageModel
    {
        private readonly IFriendRequestService _friendRequestService;
        private readonly IUserService _userService;
        private readonly IUserLocalInfoService _localUserInfoService;
		private readonly IFriendsService _friendService;
		private readonly IPushNotificationService _pushNotificationService;
		private IValidator emailValidator;
		private UserModel currentUser;

        private string error;
        public string Error
        {
            get { return error; }
            set
            {
                error = value;
                RaisePropertyChanged(nameof(Error));
				RaisePropertyChanged(nameof(ErrorIsVisible));
            }
        }

		public bool ErrorIsVisible
		{
			get { return !string.IsNullOrWhiteSpace(nameof(Error)); }
		}

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
				RaisePropertyChanged(nameof(Message));
				RaisePropertyChanged(nameof(MessageIsVisible));
            }
        }

        public bool MessageIsVisible
        {
            get { return !string.IsNullOrWhiteSpace(nameof(Message)); }
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


		public AddFriendViewModel(IFriendRequestService friendRequest,
								  IUserService userService,
								  IUserLocalInfoService localUserInfoService,
								  IFriendsService friendService,
								  IPushNotificationService pushNotificationService)
		{
			_friendRequestService = friendRequest;
			_userService = userService;
			_localUserInfoService = localUserInfoService;
			_friendService = friendService;
			_pushNotificationService = pushNotificationService;
		}
		public override void Init(object initData)
		{
			base.Init(initData);
			currentUser = _localUserInfoService.GetUser();
            emailValidator = new EmailValidation();
        }

		protected override void ViewIsDisappearing(object sender, EventArgs e)
		{
			base.ViewIsDisappearing(sender, e);
			ResetGui();			
		}

		public ICommand SendFriendRequestCommand => new Command(
			async() =>
			{
				var emailModel = new EmailModel { Email = Email };
				if (Validate(emailModel))
				{

					var findUserResult = await _userService.GetUserByEmail(Email);
					if (findUserResult.IsSucces && currentUser != null)
					{
						var user = findUserResult.Items.FirstOrDefault();
						var foundFriendResult = await _friendService.GetFriendsAsyncByUserId(currentUser.UserId);
						
						bool FriendIsFound = false;

                        var isFriend = foundFriendResult.Items?.FirstOrDefault(i => i.UserId == user.UserId);
                        FriendIsFound = isFriend != null ? true : false;

                        if (isFriend != null)
							Error = $"{user.Name} is alreddy your friend!";

						if(user.UserId == currentUser.UserId)
							Error = "Can not add your self";						
						else if(!FriendIsFound)
						{
							FriendRequestModel friendRequest = CreateFriendRequestModel(user);

							var requestResult = await _friendRequestService.AddFriendRequest(friendRequest);
							if (requestResult.IsSucces)
							{
								Email = string.Empty;
								Message = "Request has been send!";
								if (Device.RuntimePlatform == Device.Android)							
									await _pushNotificationService.SendToUser(currentUser, user.PushToken);							
							}
							else Error = requestResult.Error;
						}
					}
					else Error = findUserResult.Error;
				}
			});

		private FriendRequestModel CreateFriendRequestModel(UserModel user)
		{
			return new FriendRequestModel
			{
				Id = Guid.NewGuid().ToString(),
				FriendId = currentUser.UserId,
				UserId = user.UserId,
				Email = currentUser.Email,
				FriendName = currentUser.Name,
				IsAccepted = false,
				IsRejected = false
			};
		}
		private bool Validate(EmailModel email)
		{
            ResetFeedback();
			var validationContext = new ValidationContext<EmailModel>(email);
			var validationResult = emailValidator.Validate(validationContext);

			foreach (var error in validationResult.Errors)
			{
				if(error.PropertyName == nameof(email.Email))
				{
					Error = error.ErrorMessage;
				}
			}
			return validationResult.IsValid;
        }

		private void ResetGui()
		{
			ResetFeedback();
			Email = string.Empty;
        }

		private void ResetFeedback()
		{
            Message = string.Empty;
            Error = string.Empty;
        }


    }
}
