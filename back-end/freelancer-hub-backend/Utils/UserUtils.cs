using System.Security.Claims;
using System.Text.Json;

namespace freelancer_hub_backend.Utils
{
    public static class UserUtils
    {
        /// <summary>
        /// Retorna o userId do Supabase JWT, lendo a claim user_metadata.sub
        /// </summary>
        public static string? GetSupabaseUserId(ClaimsPrincipal user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return null;

            var userMetadataClaim = user.Claims.FirstOrDefault(c => c.Type == "user_metadata")?.Value;
            if (string.IsNullOrEmpty(userMetadataClaim))
                return null;

            try
            {
                var userMetadata = JsonSerializer.Deserialize<JsonElement>(userMetadataClaim);
                return userMetadata.GetProperty("sub").GetString();
            }
            catch
            {
                return null;
            }
        }
    }
}
