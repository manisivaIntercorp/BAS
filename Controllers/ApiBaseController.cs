using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        public string ConnectionString { get; set; }
        public readonly IConfiguration _configuration;
        public ApiBaseController(IConfiguration configuration) {
          
            this._configuration = configuration;
            this.ConnectionString = this._configuration.GetConnectionString("connection")??"";
        }
    }
}
