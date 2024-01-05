using FreshMvvm;
using Mde.Project.Mobile.CustomMasterDetail;
using Mde.Project.Mobile.Domain.Helpers;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.ViewModels
{
    public class FriendRequestViewModel : FreshBasePageModel
    {        
        private readonly IUserLocalInfoService _userInfoService;
        private readonly IFriendRequestService _friendRequestService;
        private readonly IFriendsService _friendService;
        private UserModel currentUser;
        private List<FriendRequestModel> currentRequests;

        #region Properties
        private bool friendsIsActive;

        public bool FriendsIsActive
        {
            get { return friendsIsActive; }
            set
            {
                friendsIsActive = value;
                RaisePropertyChanged(nameof(FriendsIsActive));
            }
        }

        private bool friendsRequestIsActive;

        public bool FriendsRequestIsActive
        {
            get { return friendsRequestIsActive; }
            set
            {
                friendsRequestIsActive = value;
                RaisePropertyChanged(nameof(FriendsRequestIsActive));
            }
        }

        private ObservableCollection<FriendRequestModel> friendsRequests;

        public ObservableCollection<FriendRequestModel> FriendsRequests
        {
            get { return friendsRequests; }
            set
            {
                friendsRequests = value;
                RaisePropertyChanged(nameof(FriendsRequests));
            }
        }
        #endregion

        public FriendRequestViewModel(IUserLocalInfoService userInfoService,
                                      IFriendRequestService friendRequestService,
                                      IFriendsService friendService)
        {            
            _userInfoService = userInfoService;
            _friendRequestService = friendRequestService;
            _friendService = friendService;
        }

        public async override void Init(object initData)
        {
            base.Init(initData);
            currentUser = _userInfoService.GetUser();

            if (currentUser == null)
            {
                await CoreMethods.PopToRoot(false);
                await CoreMethods.PushPageModel<LoginViewModel>();
            }
            else
            {
                await RefreshFriendRequests();
                FriendsRequestIsActive = true;
            }
        }

        private async Task RefreshFriendRequests()
        {
            var result = await _friendRequestService.GetFriendRequestByUserId(currentUser.UserId);
            if (result.IsSucces)
            {
                currentRequests = result.Items.OrderBy(r => r.FriendName).ToList();
                FriendsRequests = new ObservableCollection<FriendRequestModel>(currentRequests);
            }
            else FriendsRequests = new ObservableCollection<FriendRequestModel>();
        }

        public ICommand NavigateToFriendsCommand => new Command(
            () =>
            {
                var masterDetail = Application.Current.MainPage as CustomFreshMasterDetail;
                masterDetail.SetMainPageDetail<FriendsViewModel>();
            });

        public ICommand AddFriendCommand => new Command(
            async () =>
            {
                await CoreMethods.PushPageModel<AddFriendViewModel>();
            });

        public ICommand IgnoreRequestCommand => new Command<FriendRequestModel>(
            async (FriendRequestModel request) =>
            {                
                request.IsRejected = true;
                var result = await _friendRequestService.UpdateFriendRequest(request);
                if (result.IsSucces) await RefreshFriendRequests();

            });

        public ICommand AcceptRequestCommand => new Command<FriendRequestModel>(
      async (FriendRequestModel request) =>
      {
          request.IsAccepted = true;
          var result = await _friendRequestService.UpdateFriendRequest(request);
          if (result.IsSucces)
          {
              var userModel = new UserModel
              {
                  UserId = request.FriendId,
                  Email = request.Email,
                  Name = request.FriendName,
              };

              var friendAddetResult = await _friendService.AddFriend(currentUser.UserId, userModel);
              var senderFriendAddedResult = await _friendService.AddFriend(userModel.UserId, currentUser);
              if(friendAddetResult.IsSucces || senderFriendAddedResult.IsSucces) await RefreshFriendRequests();
          }              

      });


    }
}
