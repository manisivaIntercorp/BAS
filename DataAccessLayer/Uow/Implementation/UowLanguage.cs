using DataAccessLayer.Uow.Interface;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowLanguage: IUowLanguage
    {
        ILanguageDAL? objLanguageDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;
        IHttpContextAccessor _httpContextAccessor;

        public UowLanguage(string connectionstring, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext?.Session.GetString("DBName") != null)
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("DBName") ?? "";
                string finalConnectionString = BuildConnectionString(connectionstring, dbName);
                connection = new Microsoft.Data.SqlClient.SqlConnection(finalConnectionString);
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
                connection = new SqlConnection(finalConnectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            else
            {
                connection = new SqlConnection(connectionstring);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
        }

        // Overloaded constructor that retrieves the connection string from HttpContext (set by your middleware)
        public UowLanguage(IHttpContextAccessor httpContextAccessor) : this(
           // Try to get the connection string from HttpContext.Items.
           httpContextAccessor.HttpContext?.Items["connection"] as string
           // If not found, fallback to the configuration.
           ?? (httpContextAccessor.HttpContext?.RequestServices
                 .GetService(typeof(IConfiguration)) as IConfiguration)
                 ?.GetConnectionString("connection")
           // If still not found, throw an exception.
           ?? throw new InvalidOperationException("No connection string found in HttpContext or configuration."),
           httpContextAccessor)
        {
        }
        
        public ILanguageDAL LanguageDALRepo
        {
            get
            {
                return objLanguageDAL == null ? objLanguageDAL = new LanguageDAL(transaction) : objLanguageDAL;
            }
        }
        private string BuildConnectionString(string baseConnectionString, string dbName)
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }

        private string BuildConnectionString(string baseConnectionString, string serverName, string userID, string password, string dbName, string maxPoolSize)
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
            {
                DataSource = serverName,
                UserID = userID,
                Password = password,
                InitialCatalog = dbName,
                //MaxPoolSize = int.Parse(maxPoolSize)
            };
            return builder.ToString();
        }
        public void Commit()
        {
            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();

                objLanguageDAL = null;
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
                        //  transaction = null;
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
                disposedValue = true;
            }
        }
        ~UowLanguage()
        {
            Dispose(false);
        }
    }
}
