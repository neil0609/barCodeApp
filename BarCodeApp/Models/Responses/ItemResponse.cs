using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarCodeApp.Models.Response
{
    public class ItemResponse<T>: SuccessResponse
    {
        public T Item { get; set; }
        public string Message { get; set; }
    }
}