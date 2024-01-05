using FluentValidation;
using FreshMvvm;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Mde.Project.Mobile.Domain.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.ViewModels
{
    public class EditListItemViewModel : FreshBasePageModel
    {
        private ItemModel currentItem;
        private readonly IListService _listService;
        private readonly IFileService _fileService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUserLocalInfoService _userInfoService;
        private readonly IStorageService _storageService;
        private IValidator ItemValidation;
        private FileResult currentFileResult;
        private ListModel currentList;

        #region properties     
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
            get { return !IsBusy; }
        }

        private string pageTitle;

        public string PageTitle
        {
            get { return pageTitle; }
            set {
                pageTitle = value;
                RaisePropertyChanged(nameof(PageTitle));
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;
                RaisePropertyChanged(nameof(Image));

            }
        }

        private string nameError;

        public string NameError
        {
            get { return nameError; }
            set
            {
                nameError = value;
                RaisePropertyChanged(nameof(NameError));
                RaisePropertyChanged(nameof(NameErrorVisible));
            }
        }

        public bool NameErrorVisible
        {
            get { return !string.IsNullOrWhiteSpace(NameError); }
        }
        #endregion

        public EditListItemViewModel(IListService listService,
                                     IFileService fileService,
                                     IPushNotificationService pushNotificationService,
                                     IUserLocalInfoService userInfoService,
                                     IStorageService storageService)
        {
            _listService = listService;
            _fileService = fileService;
            _pushNotificationService = pushNotificationService;
            _userInfoService = userInfoService;
            _storageService = storageService;
        }

        public override void Init(object initData)
        {
            var item = initData as ItemModel;
            currentItem = item;
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                PageTitle = "New Item";
            }
            else
            {
                PageTitle = "Edit Item";
            }
            LoadItemState();
            ItemValidation = new ListItemValidation();
            base.Init(initData);
        }


        public ICommand OnTakePictureCommand => new Command(
          async () =>
          {
              try
              {

                  var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                  {
                      Title = Name,

                  });
                  if (result != null)
                  {
                      currentFileResult = result;
                      var stream = await currentFileResult.OpenReadAsync();
                      await SavePicture(FileSystem.CacheDirectory);
                      Image = currentFileResult.FullPath;
                  }

              }
              catch (PermissionException)
              {
                  await CoreMethods.DisplayAlert("Permission", "We need Permission to take pictures", "Oke");
              }

          });

        public ICommand OnSelectPictureCommand => new Command(
          async () =>
          {
              try
              {
                  var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                  {
                      Title = Name,

                  });
                  if (result != null)
                  {
                      currentFileResult = result;
                      var stream = await currentFileResult.OpenReadAsync();
                      await SavePicture(FileSystem.CacheDirectory);
                      Image = Device.RuntimePlatform == Device.Android?currentFileResult.FullPath:ImageSource.FromStream(()=>stream);
                  }

              }
              catch (PermissionException)
              {
                  await CoreMethods.DisplayAlert("Permission", "We need Permission to take pictures", "Oke");
              }

          });

        public ICommand OnSaveCommand => new Command(
        async () =>
        {
            IsBusy = true;
            SaveItemState();
            string token = "";
            if (Validate(currentItem))
            {
                await SaveItem();
                string topic = currentList.Name.Replace(" ", "");
                if (Device.RuntimePlatform == Device.Android)
                {
                    token = await SecureStorage.GetAsync(MyConstants.PushToken);
                    if (token != null)
                    {
                        await _pushNotificationService.UnSubscribe(topic, new List<string> { token });
                        await SendMessageToTopic();
                    }
                }
               
                var result = await _listService.GetListById(currentItem.ListId);
                if (result.IsSucces)//with popPagemodal i would get een blank page
                                    //, getting the data but wouldent load the page
                {
                    if (Device.RuntimePlatform == Device.Android && token != null)
                        await _pushNotificationService.Subscribe(topic, new List<string> { token });

                    
                    await CoreMethods.PopToRoot(false);
                    await CoreMethods.PushPageModel<SelectedListViewModel>(currentList);
                    IsBusy = false;
                }

            }

        });

        protected async override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            var result = await _listService.GetListById(currentItem.ListId);
            currentList = result.Items.FirstOrDefault();
        }

        private async Task SendMessageToTopic()
        {
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                if (currentList.IsShared)
                {
                    var userinfo = _userInfoService.GetUser();
                    if (userinfo != null) 
                    {
                        var title = "List Update";
                        var message = $"{userinfo.Name} has update the list {currentList.Name}";
                        var topic = currentList.Name.Replace(" ", "");
                        await _pushNotificationService.SendMessage(title, message, topic);
                            }
                }
                         

            }
        }


        private async Task SaveItem()
        {
            
            bool uploadSucces = false;
            if (currentList != null)
            {                
                var toDeleteItem = currentList.Items.FirstOrDefault(i => i.Id == currentItem.Id);
                if (toDeleteItem != null)
                {
                    currentList.Items.Remove(toDeleteItem);
                    currentList.Items.Add(currentItem);
                }
                else
                {
                    currentItem.Id = Guid.NewGuid().ToString();
                    currentList.Items.Add(currentItem);
                }

                if (!Image.IsEmpty && currentFileResult != null)
                    uploadSucces = await SavePicture(FileSystem.AppDataDirectory);

                await _listService.UpdateList(currentList);

            }
        }

        private async Task<bool> SavePicture(string folder)
        {
            if (!currentList.IsShared || folder == FileSystem.CacheDirectory)
            {
                var stream = await currentFileResult.OpenReadAsync();
                currentItem.ImagePath = Path.Combine(folder, currentFileResult.FileName);
                _fileService.SaveStreamToFile(currentItem.ImagePath, stream);
                return true;
            }
            else
            {               
                var result = await _storageService.UploadImage(currentList,currentItem,currentFileResult);
                currentItem.ImagePath = result.IsSucces ? result.Items.FirstOrDefault() : "";
                return result.IsSucces;
            }
        }

        private void SaveItemState()
        {            
            currentItem.Name = Name;
            currentItem.Description = Description;
        }
    

        private bool Validate(ItemModel item)
        {
            NameError = "";


            var validationContext = new ValidationContext<ItemModel>(item);
            var validationResult = ItemValidation.Validate(validationContext);
            //loop through error to identify properties
            foreach (var error in validationResult.Errors)
            {
                if (error.PropertyName == nameof(item.Name))
                {
                    NameError = error.ErrorMessage;
                }

            }

            return validationResult.IsValid;
        }
        private void LoadItemState()
        {
            Name = currentItem.Name;
            Description = currentItem.Description;
            image = currentItem.ImagePath;

        }
    }
}
