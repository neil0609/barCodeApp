using BarCodeApp.Classes.Exceptions;
using BarCodeApp.Data;
using BarCodeApp.Domain;
using BarCodeApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace BarCodeApp.Services
{
    public class UserService : BaseService
    {
        //private IUserDataService _userDataService;

        //public UserService(IUserDataService UserDataService)
        //{
        //    _userDataService = UserDataService;
        //}

        public static ApplicationUserManager GetUserManager()
        {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public static IdentityUser CreateUser(string userName, string email, string password)
        {
            ApplicationUserManager userManager = GetUserManager();

            ApplicationUser newUser = new ApplicationUser { UserName = userName, Email = email, LockoutEnabled = false };
            IdentityResult result = null;
            try
            {
                result = userManager.Create(newUser, password);
            }
            catch
            {
                throw new IdentityResultException(result);
            }

            if (result.Succeeded)
            {
                return newUser;
            }
            else
            {
                throw new IdentityResultException(result);
            }
        }

        public static bool Signin(string username, string password)
        {
            bool result = false;

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.Find(username, password);
            if (user != null)
            {
                ClaimsIdentity signin = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, signin);
                result = true;

            }
            return result;
        }

        public static bool IsUser(string UserName)
        {
            bool result = false;

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindByName(UserName);

            if (user != null)
            {
                result = true;

                // Set the EmailConfirmed value in AspNetUsers table 
                DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_UpdateEmailConfirmed"
                   , inputParamMapper: delegate (SqlParameterCollection UpdateEmailConfirmed)
                    {
                        UpdateEmailConfirmed.AddWithValue("@EmailConfirmed", result);
                        UpdateEmailConfirmed.AddWithValue("@UserName", UserName);
                    });

            }
            return result;
        }

        public static bool GetEmailConfirmed(string UserName)
        {
            bool result = false;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_GetEmailConfirmedV2"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserName", UserName);

                   //model binding
                   SqlParameter p = new SqlParameter("@EmailConfirmed", System.Data.SqlDbType.Bit);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   result = (bool)param["@EmailConfirmed"].Value;
               }
               );
            return result;
        }

        public static ApplicationUser GetUser(string username)
        {
            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindByName(username);

            return user;
        }

        public static ApplicationUser GetUserByEmail(string email)
        {
            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindByEmail(email);

            return user;
        }

        public static ApplicationUser GetUserbyUserName(string userName)
        {

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindByName(userName);

            return user;
        }

        public static ApplicationUser GetUserById(string userId)
        {

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindById(userId);

            return user;
        }

        public static bool ChangePassWord(string userId, string newPassword)
        {
            bool result = false;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
            {
                throw new Exception("You must provide a userId and a password");
            }

            ApplicationUser user = GetUserById(userId);

            if (user != null)
            {

                ApplicationUserManager userManager = GetUserManager();

                userManager.RemovePassword(userId);
                IdentityResult res = userManager.AddPassword(userId, newPassword);

                result = res.Succeeded;

            }
            return result;
        }

        //public static bool ResetPassword(ResetPasswordRequest model)
        //{

        //    if (string.IsNullOrEmpty(model.NewPassword))
        //    {
        //        throw new Exception("You must provide a password");
        //    }

        //    Guid tokenGuid = new Guid(model.TokenId);

        //    Token token = UserTokensService.Get(tokenGuid); // Given the token, Get the UserName  

        //    ApplicationUser applicationUser = GetUserbyUserName(token.UserName); // From the UserName, get UserId

        //    bool success = ChangePassWord(applicationUser.Id, model.NewPassword); // applicationUser has the email we needed to use ChangePassWord

        //    if (success)
        //    {
        //        UserTokensService.Delete(tokenGuid);
        //    }

        //    return success;
        //}

        public static bool Logout()
        {
            bool result = false;

            IdentityUser user = GetCurrentUser();

            if (user != null)
            {
                IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                result = true;
            }

            return result;
        }

        public static IdentityUser GetCurrentUser()
        {
            if (!IsLoggedIn())
                return null;
            ApplicationUserManager userManager = GetUserManager();

            IdentityUser currentUserId = userManager.FindById(GetCurrentUserId());
            return currentUserId;
        }

        public static string GetCurrentUserId()
        {
            return "17637815-6148-4d16-ac44-8fac7f3cd05f"; /*HttpContext.Current.User.Identity.GetUserId();*/
        }

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(GetCurrentUserId());
        }

        //public static List<UserSettings> GetSettings()
        //{

        //    List<UserSettings> list = null;

        //    var currentUserId = GetCurrentUserId();
        //    if (!String.IsNullOrWhiteSpace(currentUserId))
        //    {
        //        DataProvider.ExecuteCmd(GetConnection, "dbo.UserSettingValues_SelectByUserId",
        //            inputParamMapper: delegate (SqlParameterCollection paramCollection)
        //            {
        //                paramCollection.AddWithValue("@UserId", GetCurrentUserId());

        //            }, map: delegate (IDataReader reader, short set)
        //            {
        //                UserSettings item = new UserSettings();
        //                int startingIndex = 0;

        //                item.SettingId = reader.GetSafeInt32(startingIndex++);
        //                item.Value = reader.GetSafeString(startingIndex++);
        //                item.Name = reader.GetSafeString(startingIndex++);

        //                if (list == null)
        //                {
        //                    list = new List<UserSettings>();
        //                }
        //                list.Add(item);
        //            });
        //    }
        //    return list;
        //}

        //public static void UpdateSettings(UserSettingsRequest model)
        //{
        //    DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserSettingValues_Upsert",
        //        inputParamMapper: delegate (SqlParameterCollection settings)
        //        {
        //            settings.AddWithValue("@UserId", GetCurrentUserId());
        //            settings.AddWithValue("@SettingId", model.SettingId);
        //            settings.AddWithValue("@Value", model.SettingValue);
        //        });
        //}

        public static List<RolesDomain> GetRoles()
        {
            List<RolesDomain> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.AspNetRoles_SelectRoles"
                , inputParamMapper: null
                , map: delegate (IDataReader reader, short set)
                {
                    RolesDomain roles = new RolesDomain();
                    int startingIndex = 0;

                    roles.Id = reader.GetSafeString(startingIndex++);
                    roles.Name = reader.GetSafeString(startingIndex++);
                    if (list == null)
                    {
                        list = new List<RolesDomain>();
                    }
                    list.Add(roles);
                });
            return list;
        }

        public static List<NetUserDomain> GetUserList()
        {
            List<NetUserDomain> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.AspNetUser_SELECTALL"
                , inputParamMapper: null
                , map: delegate (IDataReader reader, short set)
                {
                    NetUserDomain info = new NetUserDomain();
                    int startingIndex = 0;

                    info.Id = reader.GetSafeString(startingIndex++);
                    info.Email = reader.GetSafeString(startingIndex++);
                    info.UserName = reader.GetSafeString(startingIndex++);

                    if (list == null)
                    {
                        list = new List<NetUserDomain>();
                    }
                    list.Add(info);
                });
            return list;
        }

        public static void AddRole(string id, string roleId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUserRoles_Insert"
                     , inputParamMapper: delegate (SqlParameterCollection param)
                     {
                         param.AddWithValue("@UserId", id);
                         param.AddWithValue("@RoleId", roleId);
                     });

        }

        public static void DeleteRole(string id, string roleId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUserRoles_Delete"
               , inputParamMapper: delegate (SqlParameterCollection parameterCollection)
               {
                   parameterCollection.AddWithValue("@UserId", id);
                   parameterCollection.AddWithValue("@RoleId", roleId);
               });
        }
    }
}