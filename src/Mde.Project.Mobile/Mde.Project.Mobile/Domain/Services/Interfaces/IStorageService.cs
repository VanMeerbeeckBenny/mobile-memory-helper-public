using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IStorageService
    {
        Task<ItemResultModel<string>> UploadImage(ListModel parentList, ItemModel listItemChild, FileResult imageToSave);
        Task<ItemResultModel<string>> UploadImageFromPath(ListModel parentList, ItemModel listItemChild);     
    }
}
