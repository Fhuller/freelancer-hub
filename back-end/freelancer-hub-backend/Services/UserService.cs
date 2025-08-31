using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;

namespace freelancer_hub_backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID do usuário é obrigatório.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Language = user.Language
            };
        }

        public async Task<UserDto> CreateOrGetUserAsync(string userId, UserCreateDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            // Verifica se o usuário já existe
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser != null)
            {
                return new UserDto
                {
                    Id = existingUser.Id,
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    CreatedAt = existingUser.CreatedAt
                };
            }

            // Valida os dados antes de criar
            await ValidateUserAsync(dto, userId);

            var user = new User
            {
                Id = userId,
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task UpdateUserAsync(string id, UserUpdateDto dto)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID do usuário é obrigatório.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            await ValidateUserUpdateAsync(dto, id);

            user.Name = dto.Name;
            user.Email = dto.Email;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID do usuário é obrigatório.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            await _userRepository.DeleteAsync(user);
        }

        public async Task<UserDto> GetCurrentUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return await GetUserByIdAsync(userId);
        }

        public async Task UpdateLanguageAsync(string id, string language)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.Language = language;
            await _userRepository.UpdateAsync(user);
        }

        private async Task ValidateUserAsync(UserCreateDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("O nome do usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("O email do usuário é obrigatório.");

            try
            {
                var _ = new System.Net.Mail.MailAddress(dto.Email);
            }
            catch
            {
                throw new ArgumentException("O email fornecido é inválido.");
            }

            bool emailExists = await _userRepository.EmailExistsAsync(dto.Email, userId);
            if (emailExists)
                throw new ArgumentException("Já existe um usuário com este email.");

            if (dto.Name.Length < 2)
                throw new ArgumentException("O nome deve ter pelo menos 2 caracteres.");

            if (dto.Name.Length > 100)
                throw new ArgumentException("O nome não pode ter mais de 100 caracteres.");
        }

        private async Task ValidateUserUpdateAsync(UserUpdateDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("O nome do usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("O email do usuário é obrigatório.");

            try
            {
                var _ = new System.Net.Mail.MailAddress(dto.Email);
            }
            catch
            {
                throw new ArgumentException("O email fornecido é inválido.");
            }

            bool emailExists = await _userRepository.EmailExistsAsync(dto.Email, userId);
            if (emailExists)
                throw new ArgumentException("Já existe um usuário com este email.");

            if (dto.Name.Length < 2)
                throw new ArgumentException("O nome deve ter pelo menos 2 caracteres.");

            if (dto.Name.Length > 100)
                throw new ArgumentException("O nome não pode ter mais de 100 caracteres.");
        }
    }
}