namespace WebApi.Services
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtService _jwtService;

        public JwtMiddleware(RequestDelegate next, JwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            //if (!string.IsNullOrEmpty(token))
            //{
            //    var principal = _jwtService.ValidateToken(token);

            //    if (principal != null)
            //    {
            //        context.User = principal;
            //    }
            //}

            await _next(context);
        }
    }

}
