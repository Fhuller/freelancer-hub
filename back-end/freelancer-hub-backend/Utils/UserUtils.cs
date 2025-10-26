using freelancer_hub_backend.Models;
using System.Security.Claims;
using System.Text.Json;

namespace freelancer_hub_backend.Utils
{
    public class UserUtils : IUserUtils
    {
        /// <summary>
        /// Retorna o userId do Supabase JWT, lendo a claim user_metadata.sub
        /// </summary>
        public string GetSupabaseUserId(ClaimsPrincipal user)
        {
            var nameIdentifierClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (nameIdentifierClaim == null || string.IsNullOrEmpty(nameIdentifierClaim.Value))
            {
                throw new UnauthorizedAccessException("Usuário sem ID (nameidentifier) no token.");
            }

            return nameIdentifierClaim.Value;
        }
    }
}
