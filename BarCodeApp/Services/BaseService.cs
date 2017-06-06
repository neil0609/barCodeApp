using BarCodeApp.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BarCodeApp.Services
{
    public class BaseService
    {
        protected static IDao DataProvider
        {
            get { return BarCodeApp.Data.DataProvider.Instance; }
        }

        protected static SqlConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(
                System.Web.Configuration.WebConfigurationManager
                .ConnectionStrings["DefaultConnection"].ConnectionString);
        }
    }
}