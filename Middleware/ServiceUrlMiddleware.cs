using DataAccessLayer.Model;
using Microsoft.SqlServer.Management.XEvent;

namespace WebApi.Middleware
{
    
    public class ServiceUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public ServiceUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var host = context.Request.Host.Host; // Extract domain name
            var region ="";
            var vRegionCode = context?.Session?.GetString(Common.SessionVariables.RegionCode) ?? "";
            if (!String.IsNullOrEmpty(vRegionCode))
            {
                region = vRegionCode;
            }
            else
            {
                region = GetRegionFromDomain(host);
            }
            if (!string.IsNullOrEmpty(region))
            {
                context.Items["Region"] = region;
            }
            await _next(context);
        }

        private string GetRegionFromDomain(string domain)
        {
            // Example: Extract region from subdomain (e.g., "us.example.com" -> "us")
            var parts = domain.Split('.');
            return parts.Length > 2 ? parts[0] : "default"; // Default if no subdomain
        }


    }
}
