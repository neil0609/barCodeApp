using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarCodeApp.Models.ViewModel
{
    public class BaseViewModel
    {
        public bool IsLoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public IdentityUser CurrentUser { get; set; }
    }
}