using FreshMvvm;
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
    public class SharedListViewModel : FreshBasePageModel
    {
        private readonly IFriendsService _friendsService;
        private readonly IListService _listService;
        private readonly IsharesService _shareService;
        private readonly IUserLocalInfoService _userInfo;
        private readonly IUserService _userService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IStorageService _storageService;
        private ListModel currentList;
        private List<SharedListSettings> currentShareSettings;
        private UserModel currentUser;

        private ObservableCollection<SharedListSettings> friends;

        public ObservableCollection<SharedListSettings> Friends
        {
            get { return friends; }
            set { 
                friends = value; 
                RaisePropertyChanged(nameof(Friends));
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {

                isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
                RaisePropertyChanged(nameof(ShowContent));
            }
        }
        public bool ShowContent
        {
            get { return !IsBusy; }
        }

        public SharedListViewModel(IFriendsService friendsService,
                                   IListService listService,
                                   IsharesService shareService,
                                   IUserLocalInfoService userinfo,
                                   IUserService userService,
                                   IPushNotificationService pushNotificationService,
                                   IStorageService storageService)
        {
            _friendsService = friendsService;
            _listService = listService;
            _shareService = shareService;
            _userInfo = userinfo;
            _userService = userService;
            _pushNotificationService = pushNotificationService;
            _storageService = storageService;
        }   
        public async override void Init(object initData)
        {
            base.Init(initData);
            currentList = initData as ListModel;
            currentUser = _userInfo.GetUser();
            
            if (currentUser == null) 
            {
                await CoreMethods.DisplayAlert("Not Allowed", "You most me logged in!","Oke");
                await CoreMethods.PopToRoot(false);
                await CoreMethods.PushPageModel<LoginViewModel>();
            }          
            await Refresh();
        }

        public ICommand OnSwitchToggle => new Command<SharedListSettings>(
          (SharedListSettings friend) =>
          {
              var shareToAdd = new SharedListSettings
              {
                  Id = friend.Id,
                  UserId = friend.UserId,
                  Name = friend.Name,
                  ListId = friend.ListId,
                  OwnerId = friend.OwnerId,
                  WritePermission = friend.IsShared ? friend.WritePermission : false,
                  IsShared = friend.IsShared,
              };

              var foundShare = currentShareSettings.FirstOrDefault(x => x.Id == friend.Id);
              if (foundShare != null) currentShareSettings.Remove(foundShare);
              currentShareSettings.Add(shareToAdd);
          });

        public ICommand OnCheckBoxChecked => new Command<SharedListSettings>(
            (SharedListSettings friend) =>
            {
                var shareToAdd = new SharedListSettings
                {
                    Id = friend.Id,
                    UserId = friend.UserId,
                    Name = friend.Name,
                    ListId = friend.ListId,
                    OwnerId = friend.OwnerId,
                    WritePermission = friend.IsShared ? friend.WritePermission : false,
                    IsShared = friend.IsShared,
                };


                var foundShare = currentShareSettings.FirstOrDefault(x => x.Id == friend.Id);
                if (foundShare != null) currentShareSettings.Remove(foundShare);
                currentShareSettings.Add(shareToAdd);
            });

        public ICommand OnSaveCommand => new Command(
         async () =>
         {
             IsBusy = true;
             var result = await _shareService.SaveShare(currentShareSettings);
             await UpdateList();
             await SubscribeAllShareUsers();
             await UploadPictures();
             var resultUpdate = await _listService.UpdateList(currentList);
             if (result.IsSucces && resultUpdate.IsSucces) await CoreMethods.PopPageModel();
             IsBusy = false;
         });

        private async Task UploadPictures()
        {
            if (currentList.IsShared) 
            { 
                foreach (var item in currentList.Items)
                {
                    if(item.ImagePath != null)
                    {
                        if (item.ImagePath.Contains(FileSystem.AppDataDirectory))
                        {
                            var result = await _storageService.UploadImageFromPath(currentList, item);
                            if (result.IsSucces) item.ImagePath = result.Items.FirstOrDefault();
                        }
                    }             
                
                }            
            }
        }
        private async Task SubscribeAllShareUsers()
        {
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                List<string> tokens = new List<string>();
                var shares = currentShareSettings.Where(cs => cs.IsShared == true).ToList();
                foreach (var share in shares)
                {
                    var result = await _userService.GetByIdAsync(share.UserId);
                    if (result.IsSucces)
                    {
                        UserModel user = result.Items.FirstOrDefault();
                        if(user?.PushToken != null)
                        tokens.Add(user.PushToken);
                    }

                }

                if (tokens.Any())
                {
                    string userToken = await SecureStorage.GetAsync(MyConstants.PushToken);
                    tokens.Add(userToken);
                    await _pushNotificationService.Subscribe(currentList.Name, tokens);
                }
                
            }
        }

        private async Task UpdateList()
        {
            if(currentShareSettings.Any(css => css.IsShared))
            {
                currentList.IsShared = true;
                await _listService.UpdateList(currentList);
            }else
            {
                currentList.IsShared = false;
                await _listService.UpdateList(currentList);
            }
        }
        private async Task Refresh()
        {             
            await CreateSharedFriendsSettings(currentUser);            
        }

        private async Task CreateSharedFriendsSettings(UserModel currentUser)
        {
            currentShareSettings = new List<SharedListSettings>();           
            var resultShares = await _shareService.GetSharesByListId(currentList.Id);

            if (resultShares.IsSucces)
            {
                var tempShare = resultShares.Items.ToList();
             
                var defaultFriends = await CreateNewFriendSettings();
                foreach (var share in tempShare)
                {                        
                    var foundSettings = defaultFriends.SingleOrDefault(d => d.UserId == share.UserId);
                    if (foundSettings != null)
                    {
                        defaultFriends.Remove(foundSettings);
                        defaultFriends.Add(share);
                    }
                }

                currentShareSettings = defaultFriends.OrderBy(f => f.Name).ToList();
                   
            }
            else
            {
                var defaultFriends = await CreateNewFriendSettings();
                currentShareSettings = defaultFriends.OrderBy(f => f.Name).ToList();
            }
            Friends = new ObservableCollection<SharedListSettings>(currentShareSettings);
        }

        private async Task<List<SharedListSettings>> CreateNewFriendSettings()
        {
            var result = await _friendsService.GetFriendsAsyncByUserId(currentUser.UserId);
            List<SharedListSettings> defaultFriendSettings = new List<SharedListSettings>();
            List<UserModel>friends = new List<UserModel>();
            if(result.IsSucces)friends = result.Items.ToList();
            foreach (var friend in friends)
            {
                if (friend.UserId != currentUser.UserId)
                {
                    SharedListSettings share = new SharedListSettings
                    {
                        Id = Guid.NewGuid().ToString(),
                        OwnerId = currentUser.UserId,
                        ListId = currentList.Id,
                        UserId = friend.UserId,
                        Name = friend.Name,
                        IsShared = false,
                        WritePermission = false,
                    };
                    defaultFriendSettings.Add(share);
                }
            }
            return defaultFriendSettings;
        }
    }
}
