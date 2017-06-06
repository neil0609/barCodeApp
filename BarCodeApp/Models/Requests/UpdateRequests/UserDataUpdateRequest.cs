using BarCodeApp.Models.Requests.AddRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarCodeApp.Models.Requests.UpdateRequests
{
    public class UserDataUpdateRequest : UserDataAddRequest
    {
        [Required]
        public int Id { get; set; }
    }
}