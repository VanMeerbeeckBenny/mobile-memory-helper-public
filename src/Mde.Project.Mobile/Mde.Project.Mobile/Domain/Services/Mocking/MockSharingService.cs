using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Mocking
{
    public class MockSharingService : IsharesService
    {
        private List<SharedListSettings> _Shares = new List<SharedListSettings>
        {
           
            new SharedListSettings
            {    
                Id = Guid.NewGuid().ToString(),
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                ListId ="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="PGgBqu411YX0JG6MAcOg1nGgEEg1",
                Name ="Benny",
                WritePermission = true,
                IsShared =true,
            },
              new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                ListId="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                Name = "Jeffke",
                WritePermission = true,
                IsShared=true,
            },
             new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                ListId="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzru411YX0JG6MAcOg1nGgEEg1",
                Name="Danny",
                WritePermission = true,
                IsShared=true,
            },
            new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                ListId="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzru411YX0JG6MAcOg1sgeEEg1",
                Name="Eddy",
                WritePermission = true,
                IsShared=false,
            },
            new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                ListId="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzru411YX0xRgMAcOg1nGgEEg1",
                Name="Cindy",
                WritePermission = false,
                IsShared=true,
            },
            new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "zf52BzpdYbaoKgZbX3o6dSAX73K3",
                ListId="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzruDzeYX0JG6MAcOg1nGgEEg1",
                Name="Yoran",
                WritePermission = false,
                IsShared=false,
            },
               new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                ListId ="PXgBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="XxxBqu411YX0JG6MAcOg1nGgEEg1",
                Name="Jeffke",
                WritePermission = false,
                IsShared =true,
            },
             new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzru411YX0JG6MAcOg1nGgEEg1",
                Name = "Danny",
                WritePermission = false,
                IsShared=false,
            },
            new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzru411YX0JG6MAcOg1sgeEEg1",
                Name="Eddy",
                WritePermission = true,
                IsShared=true,
            },
            new SharedListSettings
            {   
                Id = Guid.NewGuid().ToString(),
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzru411YX0xRgMAcOg1nGgEEg1",
                Name = "Cindy",
                WritePermission = false,
                IsShared=true,
            },
            new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                ListId="PXgBqu411YX0JG6MAcOg1nGgEEg1",
                UserId ="sdzruDzeYX0JG6MAcOg1nGgEEg1",
                Name = "Yoran",
                WritePermission = false,
                IsShared=false,
            },
             new SharedListSettings
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = "sdzru411YX0JG6MAcOg1sgeEEg1",
                ListId="xXgBqu411YX0JG6MAcOg1nGgEEg15",
                UserId ="PGgBqu411YX0JG6MAcOg1nGgEEg1",
                Name = "Benny",
                WritePermission = false,
                IsShared=false,
            },
        };


        public async Task<ItemResultModel<SharedListSettings>> GetSharesByListId(string listId)
        {
            var share = _Shares.Where(s => s.ListId == listId).ToList();
            ItemResultModel<SharedListSettings> result = new ItemResultModel<SharedListSettings>();
            if (!share.Any()) result.Error = "Nothing is found";
            else 
            {
                result.IsSucces = true;
                result.Items = CreateClone(share);

            }

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<SharedListSettings>> GetSharesByUserId(string userId)
        {
            var share = _Shares.Where(s => s.UserId == userId).ToList();
            ItemResultModel<SharedListSettings> result = new ItemResultModel<SharedListSettings>();
            if (share == null) result.Error = "Nothing is found";
            else
            {
                result.IsSucces = true;
                result.Items = share;

            }

            return await Task.FromResult(result);
        }


        public async Task<ItemResultModel<SharedListSettings>> GetShareSettingsOfListByUserId(string userId,string listId)
        {
            ItemResultModel<SharedListSettings> result = new ItemResultModel<SharedListSettings>();
            var shareSettingOfList =await GetSharesByListId(listId);
            if (!shareSettingOfList.IsSucces) result.Error = "No list found!";
            else
            {
                var share = shareSettingOfList.Items.SingleOrDefault(s => s.UserId == userId);

                if (share == null) result.Error = "Nothing is found";
                else
                {
                    result.IsSucces = true;
                    result.Items = new List<SharedListSettings>{ share};
                }
            }         

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<SharedListSettings>> SaveShare(List<SharedListSettings> shares)
        {            
            ItemResultModel<SharedListSettings> result = new ItemResultModel<SharedListSettings>();
            if (shares != null)
            {
                foreach (var item in shares)
                {
                    var foundShare = _Shares.FirstOrDefault(s => s.Id == item.Id);
                    if (foundShare != null)
                    {
                        _Shares.Remove(foundShare);
                        _Shares.Add(item);                        
                    }
                    else {
                        item.Id = Guid.NewGuid().ToString();
                        _Shares.Add(item);
                    }
                    result.IsSucces = true;
                }
            }
            else result.Error = "please prove a valid list" ;
            return await Task.FromResult(result);
        }

          private List<SharedListSettings> CreateClone(List<SharedListSettings> shares)
        {
            List<SharedListSettings>clonedShares  = new List<SharedListSettings>();
            foreach (var share in shares)
            {
                var clone = new SharedListSettings
                {
                    Id = share.Id,
                    OwnerId = share.OwnerId,
                    ListId = share.ListId,
                    UserId = share.UserId,
                    Name = share.Name,
                    IsShared = share.IsShared,
                    WritePermission = share.WritePermission
                };
                clonedShares.Add(clone);
            }
            return clonedShares;
        }

    }
}
