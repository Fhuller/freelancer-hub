using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using File = freelancer_hub_backend.Models.File;

namespace freelancer_hub_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly BlobStorageService _blobStorageService;
        private readonly FreelancerContext _context;

        public ProjectController(
            IProjectService projectService,
            BlobStorageService blobStorageService,
            FreelancerContext context)
        {
            _projectService = projectService;
            _blobStorageService = blobStorageService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            try
            {
                var userId = UserUtils.GetSupabaseUserId(User);
                var projects = await _projectService.GetProjectsAsync(userId);
                return Ok(projects);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(ProjectCreateDto dto)
        {
            try
            {
                var userId = UserUtils.GetSupabaseUserId(User);
                var project = await _projectService.CreateProjectAsync(userId, dto);
                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, ProjectUpdateDto dto)
        {
            try
            {
                await _projectService.UpdateProjectAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{projectId}/hours")]
        public async Task<ActionResult<ProjectHoursSummaryDto>> UpdateProjectHours(Guid projectId, UpdateProjectHoursDto dto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                    return NotFound(new { message = "Projeto não encontrado" });

                if (dto.TotalHours.HasValue)
                {
                    project.TotalHours = dto.TotalHours.Value >= 0 ? dto.TotalHours.Value : 0;
                }

                if (dto.HoursToAdd.HasValue)
                {
                    var hoursToAdd = dto.HoursToAdd.Value;
                    project.TotalHours += hoursToAdd;
                }

                if (dto.HourlyRate.HasValue)
                {
                    project.HourlyRate = dto.HourlyRate.Value >= 0 ? dto.HourlyRate.Value : project.HourlyRate;
                }

                if (project.TotalHours < 0)
                {
                    project.TotalHours = 0;
                }

                await _context.SaveChangesAsync();

                var summary = new ProjectHoursSummaryDto
                {
                    ProjectId = project.Id,
                    ProjectTitle = project.Title,
                    TotalHours = project.TotalHours,
                    HourlyRate = project.HourlyRate,
                    TotalEarned = project.TotalHours * project.HourlyRate,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao atualizar horas do projeto: {ex.Message}" });
            }
        }

        [HttpGet("{projectId}/hours-summary")]
        public async Task<ActionResult<ProjectHoursSummaryDto>> GetProjectHoursSummary(Guid projectId)
        {
            try
            {
                var project = await _context.Projects
                    .Where(p => p.Id == projectId)
                    .Select(p => new ProjectHoursSummaryDto
                    {
                        ProjectId = p.Id,
                        ProjectTitle = p.Title,
                        TotalHours = p.TotalHours,
                        HourlyRate = p.HourlyRate,
                        TotalEarned = p.TotalHours * p.HourlyRate,
                        LastUpdated = DateTime.UtcNow
                    })
                    .FirstOrDefaultAsync();

                if (project == null)
                    return NotFound(new { message = "Projeto não encontrado" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter resumo de horas: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{projectId}/files")]
        public async Task<ActionResult<FileDto>> UploadFileToProject(Guid projectId, IFormFile file)
        {
            try
            {
                // Verificar se o projeto existe
                var project = await _projectService.GetProjectByIdAsync(projectId);
                if (project == null)
                    return NotFound(new { message = "Projeto não encontrado" });

                // Validar arquivo
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Arquivo inválido" });

                if (file.Length > 10 * 1024 * 1024) // 10MB
                    return BadRequest(new { message = "Arquivo muito grande. Tamanho máximo: 10MB" });

                // Upload para Azure Blob Storage
                using var stream = file.OpenReadStream();
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var fileUrl = await _blobStorageService.UploadFileAsync(stream, fileName);

                // Criar entidade File
                var fileEntity = new File
                {
                    Id = Guid.NewGuid(),
                    FileName = Path.GetFileNameWithoutExtension(file.FileName),
                    FileExtension = Path.GetExtension(file.FileName),
                    FileUrl = fileUrl,
                    FileSize = file.Length
                };

                // Criar relação ProjectFile
                var projectFile = new ProjectFile
                {
                    ProjectId = projectId,
                    FileId = fileEntity.Id
                };

                // Salvar no banco
                await _context.Files.AddAsync(fileEntity);
                await _context.ProjectFiles.AddAsync(projectFile);
                await _context.SaveChangesAsync();

                // Retornar DTO
                var fileDto = new FileDto
                {
                    Id = fileEntity.Id,
                    FileName = fileEntity.FileName,
                    FileExtension = fileEntity.FileExtension,
                    FileUrl = fileEntity.FileUrl,
                    FileSize = fileEntity.FileSize,
                    CreatedAt = fileEntity.CreatedAt
                };

                return Ok(fileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao fazer upload do arquivo: {ex.Message}" });
            }
        }

        [HttpGet("{projectId}/files")]
        public async Task<ActionResult<IEnumerable<FileDto>>> GetProjectFiles(Guid projectId)
        {
            try
            {
                var files = await (from pf in _context.ProjectFiles
                                   join f in _context.Files on pf.FileId equals f.Id
                                   where pf.ProjectId == projectId
                                   select new FileDto
                                   {
                                       Id = f.Id,
                                       FileName = f.FileName,
                                       FileExtension = f.FileExtension,
                                       FileUrl = f.FileUrl,
                                       FileSize = f.FileSize,
                                       CreatedAt = f.CreatedAt
                                   }).ToListAsync();

                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter arquivos: {ex.Message}" });
            }
        }

        [HttpDelete("{projectId}/files/{fileId}")]
        public async Task<IActionResult> DeleteProjectFile(Guid projectId, Guid fileId)
        {
            try
            {
                var projectFile = await _context.ProjectFiles
                    .FirstOrDefaultAsync(pf => pf.ProjectId == projectId && pf.FileId == fileId);

                if (projectFile == null)
                    return NotFound(new { message = "Arquivo não encontrado no projeto" });

                var file = await _context.Files.FindAsync(fileId);
                if (file == null)
                    return NotFound(new { message = "Arquivo não encontrado" });

                // Deletar do Azure Blob Storage
                var fileName = Path.GetFileName(file.FileUrl);
                await _blobStorageService.DeleteFileAsync(fileName);

                // Deletar do banco
                _context.ProjectFiles.Remove(projectFile);
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao deletar arquivo: {ex.Message}" });
            }
        }
    }
}