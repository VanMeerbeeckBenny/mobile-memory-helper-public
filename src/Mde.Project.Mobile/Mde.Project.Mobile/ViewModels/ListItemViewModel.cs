using FreshMvvm;
using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.ViewModels
{
    public class ListItemViewModel:FreshBasePageModel
    {
		private ItemModel currentItem;

        #region Properties
        private string name;

		public string Name
		{
			get { return name; }
			set { 
				name = value;
				RaisePropertyChanged(nameof(Name));
			}
		}

		private string description;

		public string Description
		{
			get { return description; }
			set { 
				description = value; 
				RaisePropertyChanged(nameof(Description));
			}
		}

		private string imagePath;

		public string ImagePath
		{
			get { return imagePath; }
			set { 
				imagePath = value;
				RaisePropertyChanged(nameof(ImagePath));

			}
		}
        #endregion

        public override void Init(object initData)
		{
			var item = initData as ItemModel;
			currentItem = item;
			LoadItemState();
            base.Init(initData);
		}

		private void LoadItemState()
		{
            Name = currentItem.Name;
            Description = currentItem.Description;
			imagePath = currentItem.ImagePath;
            
        }

    }
}
