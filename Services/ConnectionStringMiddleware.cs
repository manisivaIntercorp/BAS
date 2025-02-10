using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using DataAccessLayer.Services;

namespace WebApi.Services
{
    public class ConnectionStringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        // public ConnectionStringMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        public ConnectionStringMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? connectionString=string.Empty;
            var session = context.Session;
            if (session == null)
            {
                await _next(context);
                return;
            }

            // Create service scope and resolve EncryptedDecrypt
            using (var scope = _serviceProvider.CreateScope())
            {
                var encryptedDecrypt = scope.ServiceProvider.GetRequiredService<EncryptedDecrypt>();

                if (session.GetString("DBName") != null && !String.IsNullOrEmpty(session.GetString("DBName")) && session.GetString("InstanceChange") != "Y")
                {
                    connectionString = BuildConnectionString(session.GetString("DBName"));
                }
                else if (session.GetString("InstanceName") != null &&
                         session.GetString("InstanceChange") == "Y" &&
                         session.GetString("DataBaseUserName") != null &&
                         session.GetString("DataBasePassword") != null)
                {
                 connectionString = "";
                    connectionString = BuildConnectionString(
                        encryptedDecrypt.Decrypt(session.GetString("InstanceName")),
                        encryptedDecrypt.Decrypt(session.GetString("DataBaseUserName")),
                        encryptedDecrypt.Decrypt(session.GetString("DataBasePassword")),
                        session.GetString("DBName"));
                }
                else
                {
                    var config = context.RequestServices.GetService<IConfiguration>();
                    connectionString = config?.GetConnectionString("connection");
                }

                // Set the connection string in HttpContext.Items
                context.Items["connection"] = connectionString;
            }

            await _next(context);
        }

        private string BuildConnectionString(string? dbName)
        {
            var config = _httpContextAccessor?.HttpContext?.RequestServices.GetService<IConfiguration>();
            var connectionString = config?.GetConnectionString("connection");
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }

        private string BuildConnectionString(string? serverName, string? userID, string? password, string? dbName)
        {
            var config = _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var connectionString = config.GetConnectionString("connection");
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                DataSource = serverName,
                UserID = userID,
                Password = password,
                InitialCatalog = dbName,
                TrustServerCertificate = true,
                MultipleActiveResultSets=true
            };
            return builder.ToString();
        }
    }
}