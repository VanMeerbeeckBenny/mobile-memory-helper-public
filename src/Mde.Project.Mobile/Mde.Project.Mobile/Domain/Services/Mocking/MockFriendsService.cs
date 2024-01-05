using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Domain.Services.Mocking
{
    public class MockFriendsService : IFriendsService
    {
        private List<FriendsModel> _friendsList = new List<FriendsModel>
        {

            new FriendsModel{          
            UserId = "PGgBqu411YX0JG6MAcOg1nGgEEg1",
            Friends = new List<UserModel> {
            new UserModel{UserId="XxxBqu411YX0JG6MAcOg1nGgEEg1" , Name="Jeffke",Email="jeffke@hotmail.com"},
            new UserModel{UserId="sdzru411YX0JG6MAcOg1nGgEEg1" , Name="Danny",Email="danny@hotmail.com"},
            new UserModel{UserId="sdzru411YX0JG6MAcOg1sgeEEg1" , Name="Eddy",Email="eddy@hotmail.com"},
            new UserModel{UserId="sdzru411YX0xRgMAcOg1nGgEEg1" , Name="Cindy",Email="cindy@hotmail.com"},
            new UserModel{UserId="sdzruDzeYX0JG6MAcOg1nGgEEg1" , Name="Yoran",Email="yoran@hotmail.com"}}
            },
            new FriendsModel{           
            UserId = "XxxBqu411YX0JG6MAcOg1nGgEEg1",
            Friends = new List<UserModel> {
            new UserModel{UserId="PGgBqu411YX0JG6MAcOg1nGgEEg1" , Name="Benny",Email="benny@hotmail.com"},
            new UserModel{UserId="sdzru411YX0JG6MAcOg1nGgEEg1" , Name="Danny",Email="danny@hotmail.com"},
            new UserModel{UserId="sdzru411YX0JG6MAcOg1sgeEEg1" , Name="Eddy",Email="eddy@hotmail.com"}}
            }
        };

        public async Task<ItemResultModel<UserModel>> GetFriendsAsyncByUserId(string userId)
        {
            ItemResultModel<UserModel> resultModel = new ItemResultModel<UserModel>();
            var result = _friendsList.SingleOrDefault(list => list.UserId == userId);
            if(result != null)
            {
                resultModel.Items = result.Friends.Select(u => new UserModel
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                }).ToList();
                resultModel.IsSucces=true;
            }
            else resultModel.IsSucces = false;

            return await Task.FromResult(resultModel);
        }

        public async Task<ItemResultModel<UserModel>> AddFriend(string userId,UserModel friend)
        {
            ItemResultModel<UserModel> result = new ItemResultModel<UserModel>();
            var userFriendsModel = _friendsList.FirstOrDefault(u => u.UserId == userId);            

            if(userFriendsModel != null)
            {
                var foundFriend = userFriendsModel.Friends.FirstOrDefault(u => u.UserId == friend.UserId);

                if (foundFriend == null)
                {
                    userFriendsModel.Friends.Add(friend);
                    result.IsSucces = true;
                }
                else result.Error = $"{friend.Name} is already a friend!";                
            }
            else
            {
                 
               var friendModel=  CreateFriendModel(userId, friend);
               _friendsList.Add(friendModel);
                
            }

            return await Task.FromResult(result);
        }

        public async Task<ItemResultModel<UserModel>> RemoveFriend(string userId, UserModel friend)
        {
            ItemResultModel<UserModel> result = new ItemResultModel<UserModel>();
         
                var userFriendsModel = _friendsList.FirstOrDefault(u => u.UserId == userId);

                if (userFriendsModel != null)
                {
                    var foundFriend = userFriendsModel.Friends.FirstOrDefault(u => u.UserId == friend.UserId);

                    if (foundFriend != null)
                    {
                        userFriendsModel.Friends.Remove(foundFriend);
                        result.IsSucces = true;
                    }
                    else result.Error = $"Friend not found!";
                }
       

            return await Task.FromResult(result);
        }

        private FriendsModel CreateFriendModel(string userId, UserModel friend)
        {
            var friendModel = new FriendsModel
            {                
                UserId = userId,
                Friends = new List<UserModel>
                        {
                            new UserModel
                            {
                                Email = friend.Email,
                                Name = friend.Name,
                                UserId = friend.UserId
                            }
                        }
            };

            return friendModel;
        }
    }

}
