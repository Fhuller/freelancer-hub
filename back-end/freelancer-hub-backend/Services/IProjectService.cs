using freelancer_hub_backend.DTO_s;

namespace freelancer_hub_backend.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsAsync(string userId);
        Task<ProjectDto> GetProjectByIdAsync(Guid id);
        Task<ProjectDto> CreateProjectAsync(string userId, ProjectCreateDto dto);
        Task UpdateProjectAsync(Guid id, ProjectUpdateDto dto);
        Task DeleteProjectAsync(Guid id);
    }
}