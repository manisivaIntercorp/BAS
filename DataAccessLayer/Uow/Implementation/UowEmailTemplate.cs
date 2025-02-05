using DataAccessLayer.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Transactions;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowEmailTemplate: IUowEmailTemplate
    {
        EmailTemplateDAL? objEmailTemplateDAL = null;
        IDbTransaction? _transaction;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string connectionString;
        private readonly IDbConnection? _connection;
        
        IDbConnection? connection = null;


        // Existing constructor that accepts connection string and IHttpContextAccessor.
        public UowEmailTemplate(string connectionstring, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext?.Session.GetString("DBName") != null)
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("DBName") ?? "";
                string finalConnectionString = BuildConnectionString(connectionstring, dbName);
                connection = new Microsoft.Data.SqlClient.SqlConnection(finalConnectionString);
                connection.Open();
                _transaction = connection.BeginTransaction();
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
                _transaction = connection.BeginTransaction();
            }
            else
            {
                connection = new SqlConnection(connectionstring);
                connection.Open();
                _transaction = connection.BeginTransaction();
            }
        }

        // Overloaded constructor that retrieves the connection string from HttpContext (set by your middleware)
        public UowEmailTemplate(IHttpContextAccessor httpContextAccessor) : this(
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
        public UowEmailTemplate(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            this.connectionString = connectionString;

            // Initialize connection and transaction
            _connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public EmailTemplateDAL EmailTemplateDALRepo
        {
            get
            {
                return objEmailTemplateDAL == null ? objEmailTemplateDAL = new EmailTemplateDAL(_transaction, connectionString) : objEmailTemplateDAL;
            }
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
            }
            finally
            {
                _transaction?.Dispose();

                objEmailTemplateDAL = null;
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
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
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
        ~UowEmailTemplate()
        {
            Dispose(false);
        }
    }
}
