using freelancer_hub_backend.DTO_s;

namespace freelancer_hub_backend.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> CreateOrGetUserAsync(string userId, UserCreateDto dto);
        Task UpdateUserAsync(string id, UserUpdateDto dto);
        Task DeleteUserAsync(string id);
        Task<UserDto> GetCurrentUserAsync(string userId);
        Task UpdateLanguageAsync(string id, string language);
    }
}