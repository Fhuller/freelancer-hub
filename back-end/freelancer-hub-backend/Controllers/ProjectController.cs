using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly FreelancerContext _context;

        public ProjectController(FreelancerContext context)
        {
            _context = context;
        }

        // GET: api/project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
        {
            var projects = await _context.Projects
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    ClientId = p.ClientId,
                    Title = p.Title,
                    Description = p.Description,
                    Status = p.Status,
                    DueDate = p.DueDate,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/project/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetById(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound();

            var dto = new ProjectDto
            {
                Id = project.Id,
                UserId = project.UserId,
                ClientId = project.ClientId,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                DueDate = project.DueDate,
                CreatedAt = project.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/project
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create(ProjectCreateDto dto)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ClientId = dto.ClientId,
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var result = new ProjectDto
            {
                Id = project.Id,
                UserId = project.UserId,
                ClientId = project.ClientId,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                DueDate = project.DueDate,
                CreatedAt = project.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, result);
        }

        // PUT: api/project/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProjectUpdateDto dto)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound();

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.Status = dto.Status;
            project.DueDate = dto.DueDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/project/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
