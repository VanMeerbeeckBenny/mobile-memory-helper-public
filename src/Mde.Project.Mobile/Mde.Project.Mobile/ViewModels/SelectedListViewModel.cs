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
using static UIKit.UIGestureRecognizer;

namespace Mde.Project.Mobile.ViewModels
{
    public class SelectedListViewModel : FreshBasePageModel
    {

        private readonly IListService _listService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUserLocalInfoService _userInfoService;
        private readonly IsharesService _shareService;
        private ListModel currentList;
        private UserModel currentUser;
        private bool hasWritePermission;

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        private ObservableCollection<ItemModel> currentItems;
        public ObservableCollection<ItemModel> CurrentItems
        {
            get { return currentItems; }
            set
            {
                currentItems = value;
                RaisePropertyChanged(nameof(CurrentItems));
            }
        }

        public SelectedListViewModel(IUserLocalInfoService userInfoService,
                                     IsharesService shareSettingService,
                                     IListService listService,
                                     IPushNotificationService pushNotificationService)
        {
            _userInfoService = userInfoService;
            _shareService = shareSettingService;
            _listService = listService;
            _pushNotificationService = pushNotificationService;
        }
        public override async void Init(object initData)
        {
            var data = initData as ListModel;
            currentList = data;
            CurrentItems = new ObservableCollection<ItemModel>(data.Items);
            Title = data.Name;
            currentUser = _userInfoService.GetUser();  
            hasWritePermission = await GetUserWritePermission();
        }

        protected async override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            await RefreshItems();
        }

        public ICommand OpenEditCommand => new Command<ItemModel>(
      async (ItemModel List) =>
      {
          if (hasWritePermission || currentUser.UserId == currentList.OwnerId)
              await CoreMethods.PushPageModel<EditListItemViewModel>(List);
          else await CoreMethods.DisplayAlert("No permission", "You have no write permission!", "oke");

      });

        public ICommand DeleteItemCommand => new Command<ItemModel>(
        async (ItemModel item) =>
        {
            if (hasWritePermission || currentUser.UserId == currentList.OwnerId)
            {
                currentItems.Remove(item);
                currentList.Items = currentItems.ToList();
                var result = await _listService.UpdateList(currentList);
                if (result.IsSucces)
                {                    
                    if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                    {
                        string topic = currentList.Name.Replace(" ", "");
                        var token = await SecureStorage.GetAsync(MyConstants.PushToken);
                        if (token != null)
                        {
                            await _pushNotificationService.UnSubscribe(topic, new List<string> { token });
                            await SendMessageToTopic(currentList);
                            await _pushNotificationService.Subscribe(topic, new List<string> { token });
                        }
                    }
                    await RefreshItems();                    
                }
                else await CoreMethods.DisplayAlert("Error", result.Error, "Oke");
            }
            else await CoreMethods.DisplayAlert("No permission", "You have no write permission!", "oke");

        });
       

        public ICommand OpenCreateCommand => new Command(
          async () =>
          {
              if (hasWritePermission || currentUser.UserId == currentList.OwnerId)
                  await CoreMethods.PushPageModel<EditListItemViewModel>(new ItemModel { ListId = currentList.Id });
              else await CoreMethods.DisplayAlert("No permission", "You have no write permission!", "oke");
          });

        public ICommand ShowItemCommand => new Command<ItemModel>(
            async (ItemModel item) =>
            {
                await CoreMethods.PushPageModel<ListItemViewModel>(item);
            });

        private async Task SendMessageToTopic(ListModel currentlist)
        {
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                if (currentlist.IsShared)
                {
                    var userinfo = _userInfoService.GetUser();
                    if (userinfo != null)
                    {
                        var title = "List Update";
                        var message = $"{userinfo.Name} has deleted an item in {currentlist.Name}";
                        var topic = currentlist.Name.Replace(" ", "");
                        await _pushNotificationService.SendMessage(title, message, topic);
                    }
                }


            }
        }

        private async Task RefreshItems()
        {
            var result = await _listService.GetListById(currentList.Id);
            if (result.IsSucces) currentList = result.Items.FirstOrDefault();
            CurrentItems = new ObservableCollection<ItemModel>(currentList.Items.OrderBy(i => i.Name));
        }

        private async Task<bool> GetUserWritePermission()
        {            
            if(currentUser != null)
            {
                var result = await _shareService.GetShareSettingsOfListByUserId(currentUser.UserId, currentList.Id);
                if (result.IsSucces)
                {
                    var permission = result.Items.FirstOrDefault();
                    return permission.WritePermission;
                }             
            }return false;         
        }
    }
}
