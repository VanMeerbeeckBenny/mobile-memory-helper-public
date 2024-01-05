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
    public class FriendsViewModel : FreshBasePageModel
    {
        private readonly IFriendsService _friendsService;
		private readonly IUserLocalInfoService _userInfoService;
		private UserModel currentUser;
		private List<UserModel> currentFriends;

        #region Properties
        private string search;

		public string Search
		{
			get { return search; }
			set {
				search = value;
				FindFriend();
				RaisePropertyChanged(nameof(Search));
			}
		}

		private bool friendsIsActive;

		public bool FriendsIsActive
		{
			get { return friendsIsActive; }
			set {
                friendsIsActive = value;
				RaisePropertyChanged(nameof(FriendsIsActive));
			}
		}

		private bool friendsRequestIsActive;		

		public bool FriendsRequestIsActive
		{
			get { return friendsRequestIsActive; }
			set {
				friendsRequestIsActive = value;
				RaisePropertyChanged(nameof(FriendsRequestIsActive));
			}
		}

		private ObservableCollection<UserModel> friends;

		public ObservableCollection<UserModel> Friends
		{
			get { return friends; }
			set {
				friends = value;
				RaisePropertyChanged(nameof(Friends));
			}
		}
        #endregion

        public FriendsViewModel(IFriendsService friendsService,
								IUserLocalInfoService userInfoService)
		{
			_friendsService = friendsService;
			_userInfoService = userInfoService;
		}

        protected async override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            FriendsRequestIsActive = false;
            FriendsIsActive = true;
            currentUser = _userInfoService.GetUser();

            if (currentUser == null)await CoreMethods.SwitchSelectedRootPageModel<LoginViewModel>();            
            else await Refresh();
		}

		private async Task Refresh()
		{
			var result = await _friendsService.GetFriendsAsyncByUserId(currentUser.UserId);
			if (result.IsSucces)
			{
				currentFriends = result.Items.OrderBy(u => u.Name).ToList();
				Friends = new ObservableCollection<UserModel>(currentFriends);

			}
		}

		public ICommand DeleteFriendCommand => new Command<UserModel>(
			async (UserModel friend) =>
			{
				var token = await SecureStorage.GetAsync(MyConstants.Token);
				if (!string.IsNullOrWhiteSpace(token))
				{
					var result = await _friendsService.RemoveFriend(currentUser.UserId, friend);		
					if(result.IsSucces) await Refresh();
				}
				else
				{
					var masterdetail = Application.Current.MainPage as CustomFreshMasterDetail;
					masterdetail.SetMainPageDetail<LoginViewModel>();
				}
			});
        public ICommand AddFriendCommand => new Command(
			async () =>
			{
				await CoreMethods.PushPageModel<AddFriendViewModel>();
			});

		public ICommand NavigateToFriendRequestCommand => new Command(
			 () =>
			{
                var masterDetail = Application.Current.MainPage as CustomFreshMasterDetail;
                masterDetail.SetMainPageDetail<FriendRequestViewModel>();
            });	   

        private void FindFriend()
		{
			var foundFriend = currentFriends.Where(f => f.Name.ToUpper().Contains(Search.ToUpper()))
											.OrderBy(f => f.Name)
											.ToList();
			Friends = new ObservableCollection<UserModel>(foundFriend);
		}

	}
}
