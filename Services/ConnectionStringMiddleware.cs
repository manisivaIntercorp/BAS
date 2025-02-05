using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace WebApi.Services
{
    public class ConnectionStringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConnectionStringMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string connectionString;
            var session = context.Session;
            if (session.GetString("DBName") != null)
            {
                connectionString = BuildConnectionString(session.GetString("DBName"));
            }
            else if (session.GetString("InstanceName") != null &&
                     session.GetString("InstanceChange") == "Y" &&
                     session.GetString("DataBaseUserName") != null &&
                     session.GetString("DataBasePassword") != null)
            {
                connectionString = BuildConnectionString(
                    session.GetString("InstanceName"),
                    session.GetString("DataBaseUserName"),
                    session.GetString("DataBasePassword"),
                    session.GetString("DBName"));
            }
            else
            {
                var config = context.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                // Make sure "connection" exists in your appsettings.json
                connectionString = config?.GetConnectionString("connection");
            }

            // Set the connection string in HttpContext.Items for later retrieval.
            context.Items["connection"] = connectionString;
            await _next(context);
        }

        private string BuildConnectionString(string dbName)
        {
            var config = _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var connectionString = config.GetConnectionString("connection");
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }

        private string BuildConnectionString(string serverName, string userID, string password, string dbName)
        {
            var config = _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var connectionString = config.GetConnectionString("DefaultConnection");
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                DataSource = serverName,
                UserID = userID,
                Password = password,
                InitialCatalog = dbName
            };
            return builder.ToString();
        }
    }

}