using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using FreshMvvm;
using System.Collections.ObjectModel;
using Mde.Project.Mobile.Domain.Models;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using Mde.Project.Mobile.Pages;
using Mde.Project.Mobile.Domain.Services.Local;
using Firebase.Auth;

namespace Mde.Project.Mobile.ViewModels
{
    public class MainViewModel: FreshBasePageModel
    {
        private readonly IAccountService _accountService;
        private readonly IUserLocalInfoService _localUserService;
        private readonly IsharesService _shareService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUserService _userService;
        private UserModel currentUser;

        private bool isBussy;

        public bool IsBussy
        {
            get { return isBussy; }
            set { isBussy = value; }
        }

        private ObservableCollection<ListModel> lists;

        public ObservableCollection<ListModel> Lists
        {
            get { return lists; }
            set {
                lists = value;
                RaisePropertyChanged(nameof(Lists));
            }
        }

      
        private readonly IListService _listService;
        public MainViewModel(IListService listService,
                              IAccountService accountService,
                              IUserLocalInfoService userLocalInfoService,
                              IsharesService shareService,
                              IPushNotificationService pushNotificationService,
                              IUserService userService
            )
        {
            _listService = listService;
            _accountService = accountService;
            _localUserService = userLocalInfoService;
            _shareService = shareService;
            _pushNotificationService = pushNotificationService;
            _userService = userService;
        }

        protected async override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);          
            await CheckUserIsLoggedIn();     
        }

        public ICommand OpenEditCommand => new Command<ListModel>(
          async (ListModel list) =>
          {
              if (currentUser.UserId == list.OwnerId) await CoreMethods.PushPageModel<EditListViewModel>(list);
              else await CoreMethods.DisplayAlert("No permission", "You have no permission to edit!", "Oke");
          });

        public ICommand OpenCreateCommand => new Command(
           async () =>
           {
               await CoreMethods.PushPageModel<EditListViewModel>(new ListModel());
           });

        public ICommand ShareListCommand => new Command<ListModel>(
        async (ListModel list) =>
        {
            if (list.OwnerId != currentUser.UserId)
            {
                await CoreMethods.DisplayAlert("Not Allowed", "You must own a list to be able to share!", "Oke");
            }
            else await CoreMethods.PushPageModel<SharedListViewModel>(list);

        });


        public ICommand DeleteListCommand => new Command<ListModel>(
           async (ListModel list) =>
           {
               if (list.OwnerId != currentUser.UserId)
               {
                   await CoreMethods.DisplayAlert("Not Allowed", "Can not delete a list you don't own!", "Oke");
               }
               else
               {
                   var result = await _listService.GetListById(list.Id);
                   if (result.IsSucces)
                   {
                       var item = result.Items.FirstOrDefault();
                       if (item.Items.Any())
                       {
                           var answer = await CoreMethods.DisplayAlert("Delete", "This list is not empty, are you sure to delete?", "Yes", "No");
                           if (answer == true) result = await _listService.DeleteList(list.Id);
                       }
                       else await _listService.DeleteList(list.Id);
                       await RefreshLists();
                   }
                   else { await CoreMethods.DisplayAlert("Error", "Nothing is found?", "Oke"); }
               }

           });

        public ICommand NavigateToSelectListCommand => new Command<ListModel>(
            async (ListModel list) =>
            {
                await CoreMethods.PushPageModel<SelectedListViewModel>(list);
            });

        private async Task CheckUserIsLoggedIn()
        {
            currentUser = _localUserService.GetUser();

            if (currentUser != null) 
            {
                await RefreshLists();
            } 

            else
            {
                string token = await SecureStorage.GetAsync(MyConstants.Token);
                var result = await _accountService.GetUser(token);
                
                if (result.IsSucces) 
                {
                    currentUser = result.User;             
                    await RefreshLists();
                } 
                else await CoreMethods.SwitchSelectedRootPageModel<LoginViewModel>();
            }
        }

        private async Task RefreshLists()
        {
            IsBussy = true;
            List<SharedListSettings> share = new List<SharedListSettings>();
            try
            {
                List<ListModel> items = new List<ListModel>();
                var result = await _listService.GetListbyUserId(currentUser.UserId);
                if (result.IsSucces) items.AddRange(result.Items);
                var shareResult = await _shareService.GetSharesByUserId(currentUser.UserId);
                if (shareResult.IsSucces) share = shareResult.Items.ToList();
                foreach (var item in share)
                {
                    result = await _listService.GetListById(item.ListId);
                    if (result.IsSucces) items.Add(result.Items.FirstOrDefault());
                }                
                Lists = new ObservableCollection<ListModel>(items.OrderBy(i => i.Name));
            }
            catch (Exception ex)
            {

                await CoreMethods.DisplayAlert("Error", $"{ex.Message}", "Ok");
            }
            finally
            {
                IsBussy = false;
            }
           
        }      
    }
}
