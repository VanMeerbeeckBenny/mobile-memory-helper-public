using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Mde.Project.Mobile.Domain.Models
{
    public class ItemModel : BaseModel
    {
        public string ListId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
    }
}
