using BarCodeApp.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCodeApp.Data
{
    public sealed class DataProvider
    {
        private DataProvider() { }

        public static IDao Instance
        {
            get
            {
                return SqlDao.Instance;
            }
        }

        public static void ExecuteNonQuery(object getConnection, string v, Func<SqlParameterCollection, object> inputParamMapper, Func<SqlParameterCollection, object> returnParameters)
        {
            throw new NotImplementedException();
        }
    }
}
