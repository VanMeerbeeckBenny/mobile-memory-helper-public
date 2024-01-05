using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Models
{
    public class FriendRequestModel :BaseModel
    {
        public string FriendId { get; set; }
        public string FriendName { get; set; }
        public string UserId { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public string Email { get; set; }
    }
}
