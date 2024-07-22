using System.Security.Claims;

namespace MultiShop.WebUI.Services
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public LoginService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetUserId
        {
            get
            {
                var httpContext = _contextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        return userIdClaim.Value;
                    }
                }
                return null;
            }
        }
    }
}
