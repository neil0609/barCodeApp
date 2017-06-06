using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarCodeApp.Models.Requests.AddRequests
{
    public class UserDataAddRequest
    {

        [StringLength(50, MinimumLength = 2)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string PhoneNumber { get; set; }
    }
}