using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Models
{
    public class AuthenticationResultModel
    {
        public bool IsSucces { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public UserModel User { get; set; }
    }
}
