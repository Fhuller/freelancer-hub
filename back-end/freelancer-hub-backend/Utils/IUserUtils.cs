using System.Security.Claims;

namespace freelancer_hub_backend.Utils
{
    public interface IUserUtils
    {
        string GetJWTUserID(ClaimsPrincipal user);
    }
}
