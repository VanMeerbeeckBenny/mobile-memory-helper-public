using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Models
{
    public class SharedListSettings : BaseModel
    {
        public string OwnerId { get; set; }
        public string ListId { get; set; }
        public string UserId { get; set; }
        public bool WritePermission { get; set; }
        public bool IsShared { get; set; }
        public string Name { get; set; }
    }
}
