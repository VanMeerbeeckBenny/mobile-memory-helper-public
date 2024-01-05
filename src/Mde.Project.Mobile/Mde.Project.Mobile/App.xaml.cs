using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Mde.Project.Mobile.Pages;
using FreshMvvm;
using Mde.Project.Mobile.ViewModels;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Mde.Project.Mobile.Domain.Services.Mocking;
using Xamarin.Essentials;
using Mde.Project.Mobile.CustomMasterDetail;
using Mde.Project.Mobile.Domain.Helpers;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Local;
using Mde.Project.Mobile.Domain.Services.Api;
using Plugin.FirebasePushNotification;

namespace Mde.Project.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            FreshIOC.Container.Register<IListService>(new FirebaseListService());
            FreshIOC.Container.Register<IFileService>(new FileService());
            FreshIOC.Container.Register<IAccountService>(new AccountService());
            FreshIOC.Container.Register<IUserLocalInfoService>(new UserLocalInfoService());
            FreshIOC.Container.Register<IFriendsService>(new FirebaseFriendService());
            FreshIOC.Container.Register<IsharesService>(new FirebaseShareService());
            FreshIOC.Container.Register<IFriendRequestService>(new FirebaseFriendRequestService());
            FreshIOC.Container.Register<IUserService>(new FirebaseUserService());
            FreshIOC.Container.Register<IPushNotificationService>(new FirebasePushNotificationService());
            FreshIOC.Container.Register<IStorageService>(new FirebaseStorageService());
            var masterDetailNav = new CustomFreshMasterDetail();
            masterDetailNav.Init("Menu");
            masterDetailNav.AddPage<MainViewModel>(new MenuItemModel { Title = "Home", Icon = FontAwesomeHelper.House });
            masterDetailNav.AddPage<LoginViewModel>(new MenuItemModel { Title = "Login", Icon = FontAwesomeHelper.User });
            masterDetailNav.AddPage<FriendsViewModel>(new MenuItemModel { Title = "Social", Icon = FontAwesomeHelper.UserGroup });
            masterDetailNav.Flyout.BackgroundColor = ColorConverters.FromHex("#363c3f");


            if (DeviceInfo.Platform.ToString() == Device.UWP)
            {
                masterDetailNav.Flyout.BackgroundColor = ColorConverters.FromHex("#4d565b");
            }


            MainPage = masterDetailNav;
        }
        

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
