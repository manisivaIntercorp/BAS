namespace WebApi.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //  Set Session Variable
        public void SetSession(string key, string value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        }

        //  Get Session Variable
        public string GetSession(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(key);
        }

        //  Remove Session Variable
        public void RemoveSession(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }
    }

}
