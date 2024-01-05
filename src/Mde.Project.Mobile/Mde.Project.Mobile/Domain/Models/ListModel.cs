using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Models
{
    public class ListModel:BaseModel
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public List<ItemModel> Items { get; set; }        
        public bool IsShared { get; set; }
    }
}
