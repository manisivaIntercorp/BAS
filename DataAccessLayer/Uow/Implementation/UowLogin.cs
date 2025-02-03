using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using DataAccessLayer.Uow.Interface;
using System.Data;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.SqlServer.Management.XEvent;
using Microsoft.SqlServer.Management.Smo;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowLogin : IUowLogin
    {
        private ILoginDAL? objLoginDal = null;
        private readonly IDbTransaction transaction;
        private readonly IDbConnection? connection = null;
        private readonly IHttpContextAccessor _httpContextAccessor;

         

        public UowLogin(string connectionstring,IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext?.Session.GetString("DBName") != null)
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("DBName") ?? "";
                string finalConnectionString = BuildConnectionString(connectionstring, dbName);
                connection = new SqlConnection(finalConnectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            else if (_httpContextAccessor.HttpContext?.Session != null &&
            _httpContextAccessor.HttpContext?.Session.GetString("InstanceName") != null &&
            _httpContextAccessor.HttpContext?.Session.GetString("InstanceChange") == "Y" &&
            _httpContextAccessor.HttpContext?.Session.GetString("DataBaseUserName") != null &&
            _httpContextAccessor.HttpContext?.Session.GetString("DataBasePassword") != null)
            {

                string serverName = _httpContextAccessor.HttpContext?.Session?.GetString("InstanceName") ?? "";
                string userId = _httpContextAccessor.HttpContext?.Session?.GetString("DataBaseUserName") ?? "";
                string password = _httpContextAccessor.HttpContext?.Session?.GetString("DataBasePassword") ?? "";
                string dbName = _httpContextAccessor.HttpContext?.Session?.GetString("DBName") ?? "";
                string maxPoolSize = _httpContextAccessor.HttpContext?.Session?.GetString("MaxPoolSize") ?? "100";

                string finalConnectionString = BuildConnectionString(connectionstring, serverName, userId, password, dbName, maxPoolSize);
                connection = new Microsoft.Data.SqlClient.SqlConnection(finalConnectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            else
            {
                connection = new Microsoft.Data.SqlClient.SqlConnection(connectionstring);
                connection.Open();
                transaction = connection.BeginTransaction();
            }

        }

        public ILoginDAL LoginDALRepo
        {
            get
            {
                return objLoginDal == null ? objLoginDal = new LoginDAL(transaction) : objLoginDal;
            }
        }
        private string BuildConnectionString(string baseConnectionString, string dbName)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }
        private string BuildConnectionString(string baseConnectionString, string serverName, string UserID, string password, string dbName, string maxPoolSize)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                DataSource = serverName,
                UserID = UserID,
                Password = password,
                InitialCatalog = dbName,
                //MaxPoolSize = int.Parse(maxPoolSize)
            };

            return builder.ToString();
        }
        public void commit()
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
                objLoginDal = null;
            }
        }
        private bool disposedValue = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                       // connection = null;
                    }
                }
                disposedValue = true;
            }
        }
        ~UowLogin()
        {
            Dispose(false);
        }
    }
}
