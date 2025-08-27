using freelancer_hub_backend.Models;
using System.Security.Claims;
using System.Text.Json;

namespace freelancer_hub_backend.Utils
{
    public static class UserUtils
    {
        /// <summary>
        /// Retorna o userId do Supabase JWT, lendo a claim user_metadata.sub
        /// </summary>
        public static string GetSupabaseUserId(ClaimsPrincipal user)
        {
            var userMetadataClaim = user.Claims.FirstOrDefault(c => c.Type == "user_metadata")?.Value;
            if (string.IsNullOrEmpty(userMetadataClaim))
                throw new UnauthorizedAccessException("Usuário sem metadata");

            var userMetadata = JsonSerializer.Deserialize<JsonElement>(userMetadataClaim);
            var userReturn = userMetadata.GetProperty("sub").GetString();

            if (string.IsNullOrEmpty(userReturn))
                throw new UnauthorizedAccessException("Usuário sem sub");

            return userReturn;
        }
    }
}
