using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarCodeApp.Models.Response
{
    public class SuccessResponse: BaseResponse
    {
        public SuccessResponse()
        {
            this.IsSuccessfull = true;
        }
    }
}