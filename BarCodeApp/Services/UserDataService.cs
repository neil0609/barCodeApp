using BarCodeApp.Domain;
using BarCodeApp.Models.Requests.AddRequests;
using BarCodeApp.Models.Requests.UpdateRequests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BarCodeApp.Services
{
    public class UserDataService : BaseService
    {
        public static int InsertUserDataItem(UserDataAddRequest model, string UserId)
        {
            var id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserData_Insert"
                , inputParamMapper: delegate (SqlParameterCollection AddUserData)
                {
                    AddUserData.AddWithValue("@FirstName", model.FirstName);
                    AddUserData.AddWithValue("@LastName", model.LastName);
                    AddUserData.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    AddUserData.AddWithValue("@UserId", UserId);

                    SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    p.Direction = System.Data.ParameterDirection.Output;

                    AddUserData.Add(p);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    int.TryParse(param["@Id"].Value.ToString(), out id);
                });
            return id;
        }

        public static void UpdateUserDataItem(UserDataUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserData_Update"
                , inputParamMapper: delegate (SqlParameterCollection UpdateUserData)
                {
                    UpdateUserData.AddWithValue("@Id", model.Id);
                    UpdateUserData.AddWithValue("@FirstName", model.FirstName);
                    UpdateUserData.AddWithValue("@LastName", model.LastName);
                    UpdateUserData.AddWithValue("@PhoneNumber", model.PhoneNumber);
                });
        }

        public static UserDataItem GetUserDataItem(int id)
        {
            UserDataItem item = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.UserData_SelectById"
                , inputParamMapper: delegate (SqlParameterCollection UserDataCollection)
                {
                    UserDataCollection.AddWithValue("@Id", id);
                }, map: delegate (IDataReader reader, short set)
                {
                    item = MapUserData(reader);
                });
            return item;
        }

        public static List<UserDataItem> GetUserDataItemList()
        {
            List<UserDataItem> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.UserData_SelectAll",
                inputParamMapper: null, map: delegate (IDataReader reader, short set)
                {

                    UserDataItem item = MapUserData(reader);

                    if (list == null)
                    {
                        list = new List<UserDataItem>();
                    }
                    list.Add(item);
                }
            );
            return list;
        }

        public static UserDataItem MapUserData(IDataReader reader)
        {
            UserDataItem item = new UserDataItem();
            int startingindex = 0;

            item.Id = reader.GetInt32(startingindex++);
            item.FirstName = reader.GetString(startingindex++);
            item.LastName = reader.GetString(startingindex++);
            item.PhoneNumber = reader.GetString(startingindex++);

            return item;
        }

        public static void DeleteUserDataItem(int id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserData_DeleteById"
                , inputParamMapper: delegate (SqlParameterCollection deleteUserDataItem)
                {
                    deleteUserDataItem.AddWithValue("@Id", id);
                });
        }
    }
}