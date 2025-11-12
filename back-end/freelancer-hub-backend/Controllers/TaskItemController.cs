using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace freelancer_hub_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemController : ControllerBase
    {
        private readonly IUserUtils _userUtils;
        private readonly ITaskItemService _taskItemService;

        public TaskItemController(ITaskItemService taskItemService, IUserUtils userUtils)
        {
            _taskItemService = taskItemService;
            _userUtils = userUtils;
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTaskItemsByProject(Guid projectId)
        {
            try
            {
                var userId = _userUtils.GetJWTUserID(User);
                var taskItems = await _taskItemService.GetTaskItemsByProjectAsync(userId, projectId);
                return Ok(taskItems);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskItemById(Guid id)
        {
            var taskItem = await _taskItemService.GetTaskItemByIdAsync(id);
            if (taskItem == null) return NotFound();

            return Ok(taskItem);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskItem(TaskItemCreateDto dto)
        {
            try
            {
                var userId = _userUtils.GetJWTUserID(User);
                var taskItem = await _taskItemService.CreateTaskItemAsync(userId, dto);
                return CreatedAtAction(nameof(GetTaskItemById), new { id = taskItem.Id }, taskItem);
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
        public async Task<IActionResult> UpdateTaskItem(Guid id, TaskItemUpdateDto dto)
        {
            try
            {
                await _taskItemService.UpdateTaskItemAsync(id, dto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(Guid id)
        {
            try
            {
                await _taskItemService.DeleteTaskItemAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}