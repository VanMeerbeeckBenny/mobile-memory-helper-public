using Firebase.Storage;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.Domain.Services.Api
{
    public class FirebaseStorageService : IStorageService
    {      
        public async Task<ItemResultModel<string>> UploadImage(ListModel parentList,ItemModel listItemChild, FileResult imageToSave)
        {
            var result = new ItemResultModel<string>();
            try
            {
                var task = new FirebaseStorage("firebasestoragelink",
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true,
                    })
                    .Child(parentList.Name.Replace(" ",""))
                    .Child(listItemChild.Name.Replace(" ", ""))
                    .PutAsync(await imageToSave.OpenReadAsync());

                string imageLink = await task;
                result.Items = new List<string> { imageLink };
                result.IsSucces = true;
                return result;
            }
            catch (FirebaseStorageException ex)
            {
                result.Error = ex.InnerException.Message;
                return result;
            }            
          
        }

        public async Task<ItemResultModel<string>> UploadImageFromPath(ListModel parentList, ItemModel listItemChild)
        {
            var result = new ItemResultModel<string>();
            
            try
            {

                FileStream stream = File.OpenRead(listItemChild.ImagePath);
                var imagePath =await new FirebaseStorage("firebasestoragelink",
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true,
                    })
                    .Child(parentList.Name.Replace(" ", ""))
                    .Child(listItemChild.Name.Replace(" ", ""))
                    .PutAsync(stream);

                                
                result.Items = new List<string> { imagePath };
                result.IsSucces = true;
                return result;
            }
            catch (FirebaseStorageException FirebaseEx)
            {
                result.Error = FirebaseEx.InnerException.Message;
                return result;
            }
            catch(Exception ex)
            {
                result.Error = "Picture not found";
                return result;
            }           

        }

    }
}
