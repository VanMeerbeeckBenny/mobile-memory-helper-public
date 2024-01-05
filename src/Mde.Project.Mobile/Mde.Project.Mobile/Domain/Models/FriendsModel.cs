using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Models
{
    public class FriendsModel
    {
        public string UserId { get; set; }
        public List<UserModel> Friends { get; set; }
    }
}
