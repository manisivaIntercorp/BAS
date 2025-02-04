using DataAccessLayer.Uow.Interface;

using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.AspNetCore.Http;


namespace DataAccessLayer.Uow.Implementation
{
    public class UowUserAccount: IUowUserAccount
    {
        IUserAccountDAL? objUserAccountDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UowUserAccount(string connectionstring, IHttpContextAccessor httpContextAccessor)
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
        
        public IUserAccountDAL UserAccountDALRepo
        {
            get
            {
                return objUserAccountDAL ?? new UserAccountDAL(transaction);
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
        private string BuildConnectionString(string baseConnectionString, string serverName, string UserID, string password, string dbName, string maxPoolSize)
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
            {
                DataSource = serverName,
                UserID = UserID,
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

                objUserAccountDAL = null;
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
        ~UowUserAccount()
        {
            Dispose(false);
        }
    }
}
