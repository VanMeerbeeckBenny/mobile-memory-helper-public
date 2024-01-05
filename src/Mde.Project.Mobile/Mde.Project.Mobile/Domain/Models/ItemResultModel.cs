using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Models
{
    public class ItemResultModel<T> where T : class
    {
        public bool IsSucces { get; set; }
        public string Error { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
