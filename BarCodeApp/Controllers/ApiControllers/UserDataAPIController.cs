using BarCodeApp.Domain;
using BarCodeApp.Models.Requests.AddRequests;
using BarCodeApp.Models.Requests.UpdateRequests;
using BarCodeApp.Models.Response;
using BarCodeApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BarCodeApp.Controllers.ApiControllers
{
    [AllowAnonymous]
    [RoutePrefix("api/UserData")]
    public class UserDataApiController : ApiController
    {

        [Route, HttpPost]
        public HttpResponseMessage AddUserDataItem(UserDataAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            try
            {
                ItemResponse<int> response = new ItemResponse<int>();
                string UserId = UserService.GetCurrentUserId();
                response.Item = UserDataService.InsertUserDataItem(model, UserId);
                return Request.CreateResponse(response);
            }
            catch (Exception ex)
            {
                ErrorResponse er = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, er);
            }
        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage UpdateUserDataItem(UserDataUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                SuccessResponse response = new SuccessResponse();
                UserDataService.UpdateUserDataItem(model);
                return Request.CreateResponse(response);
            }
            catch (Exception ex)
            {
                ErrorResponse er = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, er);
            }
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetUserDataItem(int id)
        {
            try
            {
                ItemResponse<UserDataItem> response = new ItemResponse<UserDataItem>();
                response.Item = UserDataService.GetUserDataItem(id);
                return Request.CreateResponse(response);
            }
            catch (Exception ex)
            {
                ErrorResponse er = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, er);
            }
        }
    }
}