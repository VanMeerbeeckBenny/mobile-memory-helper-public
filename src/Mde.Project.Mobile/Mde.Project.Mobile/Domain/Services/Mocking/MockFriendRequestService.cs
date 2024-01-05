using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Mocking
{
    public class MockFriendRequestService : IFriendRequestService
    {
        private List<FriendRequestModel> _friendRequestModels = new List<FriendRequestModel> {

            new FriendRequestModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                FriendId = "PGgBqu411YXdsqdsqdg1nGgEEg1",
                FriendName = "Joske",
                Email = "Joske@hotmail.com"
            },
            new FriendRequestModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                FriendId = "PGgBqu411YXds157dg1nGgEEg1",
                FriendName = "Siefried",
                Email = "Siefried@hotmail.com"
            },
            new FriendRequestModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                FriendId = "PGgBqud5sq4s5s7sqdg1nGgEEg1",
                FriendName = "Jurgen",
                Email = "Jurgen@hotmail.com"
            },
            new FriendRequestModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                FriendId = "15s5u411YXds157dg1nGgEEg1",
                FriendName = "Wesly",
                Email = "Wesly@hotmail.com"
            },
            new FriendRequestModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
                FriendId = "h5f6f8y5sq4s5s7sqdg1nGgEEg1",
                FriendName = "Yordi",
                Email = "Yordi@hotmail.com"
            }

        };

        public async Task<ItemResultModel<FriendRequestModel>>GetFriendRequestByUserId(string id)
        {
            var result = new ItemResultModel<FriendRequestModel>();
            var requests = _friendRequestModels.Where(x => x.UserId == id &&
                                                          x.IsAccepted == false &&
                                                          x.IsRejected == false ).ToList();

            if (requests.Any())
            {
                
                result.Items = CreateClone(requests);
                result.IsSucces = true;
            }
            else
            {
                result.Error = "Nothing is found!";               
            }

            return await Task.FromResult(result);
        }   

        public async Task<ItemResultModel<FriendRequestModel>> UpdateFriendRequest(FriendRequestModel friendRequest)
        {
            var result = new ItemResultModel<FriendRequestModel>();

            var foundRequest = _friendRequestModels.FirstOrDefault(f => f.Id == friendRequest.Id);
            if(foundRequest != null)
            {
                _friendRequestModels.Remove(foundRequest);
                _friendRequestModels.Add(friendRequest);
                result.IsSucces = true;
            }
            else
            {
                result.Error = "Request not found!";
            }

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<FriendRequestModel>> AddFriendRequest(FriendRequestModel friendRequest)
        {
            var result = new ItemResultModel<FriendRequestModel>();

            var foundRequest = _friendRequestModels.FirstOrDefault(f => f.UserId == friendRequest.UserId &&
                                                                        f.FriendId == friendRequest.FriendId &&
                                                                        f.IsRejected == false &&
                                                                        f.IsAccepted == false);
            if (foundRequest == null)
            {
                
                _friendRequestModels.Add(friendRequest);
                result.IsSucces = true;
            }
            else
            {
                result.Error = "Request exists and is stil awaited!";
            }

            return await Task.FromResult(result);
        }

        private List<FriendRequestModel> CreateClone(List<FriendRequestModel> requests)
        {
            var items = requests.Select(x => new FriendRequestModel
            {
                Id = x.Id,
                UserId = x.UserId,
                FriendId = x.FriendId,
                FriendName = x.FriendName,
                IsAccepted = x.IsAccepted,
                IsRejected = x.IsRejected,
                Email = x.Email,
            }).ToList();

            return items;
        }

    }
}
