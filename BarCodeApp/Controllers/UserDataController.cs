using BarCodeApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarCodeApp.Controllers
{
    [RoutePrefix("user")]
    public class UserDataController : Controller
    {
        [Route("add")]
        [Route("edit/{id:int}")]
        public ActionResult Index(int id = 0)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
    }
}