using FluentValidation;
using FreshMvvm;
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
    public class EditListViewModel:FreshBasePageModel
    {
		private readonly IListService _listService;
        private readonly IUserLocalInfoService _userInfoService;
        private IValidator listValidation;
        private ListModel CurrentList;

        #region properties
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
			set { 
				name = value; 
				RaisePropertyChanged(nameof(Name));
			}
		}

		private string updateMessage;

		public string UpdateMessage
		{
			get { return updateMessage; }
			set { updateMessage = value; }
		}

        private string nameError;
        private UserModel currentUser;

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

        public EditListViewModel(IListService listService,IUserLocalInfoService userInfoService)
		{
			_listService = listService;
            _userInfoService = userInfoService;		
		}

		public override void Init(object initData)
        {

            ListModel item = initData as ListModel;
            CurrentList = item;
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                PageTitle = "New List";
            }
            else
            {
                PageTitle = "Edit List";
            }
            currentUser = _userInfoService.GetUser();
            LoadItemState();
            listValidation = new ListValidation();
            base.Init(initData);

        }

        public ICommand SaveChangesCommand => new Command(
          async () =>
          {

              SaveItemState();
              if (Validate(CurrentList))
              {
                  if (string.IsNullOrWhiteSpace(CurrentList.Id))
                  {
                      CurrentList.OwnerId = currentUser.UserId;
                      CurrentList.Id = Guid.NewGuid().ToString();
                      CurrentList.IsShared = false;
                      var result = await _listService.CreateList(CurrentList);
                      if (result.IsSucces) await CoreMethods.PopPageModel();
                  }
                  else
                  {
                      var result = await _listService.UpdateList(CurrentList);
                      if (result.IsSucces) await CoreMethods.PopPageModel();
                  }
              }

          });

        private bool Validate(ListModel list)
        {
            NameError = "";      


            var validationContext = new ValidationContext<ListModel>(list);
            var validationResult = listValidation.Validate(validationContext);
            //loop through error to identify properties
            foreach (var error in validationResult.Errors)
            {
                if (error.PropertyName == nameof(list.Name))
                {
                    NameError = error.ErrorMessage;
                }
                
            }

            return validationResult.IsValid;
        }
        private void LoadItemState()
		{
			Name = CurrentList.Name;			
		}

		private void SaveItemState()
		{
			CurrentList.Name = Name;
		}
	}
}
